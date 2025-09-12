using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;


public class ControllerPlayObjekLevel3 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "PilihanLevel";

    [Header("Gameplay Manager")]
    public int Level = 3;
    public GameObject ManagerGameplay; // manager gameplay
    public GameObject ParentObjekGameplay;
    public GameObject ParentKarakter; // parent karakter

    [Header("List Objek Gameplay")]
    public List<GameObject> ListGrupObjekGameplay; // list objek gameplay

    [Header("OnClickError Object")]
    public GameObject OnClickErrorObjek; // objek OnClickError

    [Header("Window")]
    public GameObject WindowObjekParent; // objek window
    public GameObject ShadowBackground; // objek shadow background
    public GameObject WindowWinner; // objek window menang
    public GameObject WindowComplateLevel; // objek window complete level
    public GameObject AmountKoinText; // objek text jumlah koin

    [Header("UI")]
    public GameObject AmountKoinPlayer; // objek text jumlah koin player

    [Header("Gameplay Objects")]
    public GameObject BackgroundCanvas; // Background canvas
    public GameObject BackgroundOpsional; // Background gameplay
    public GameObject BackgroundOpsional2; // Background gameplay
    public List<GameObject> ListGrupGameplayObject; // list objek setelah gameplay

    [Header("Level 3 - Progress 7")]
    public GameObject KarakterLakiBaju;
    public GameObject KarakterPerempuanBaju;
    public GameObject HandKenakanBajuP;
    public GameObject HandKenakanBajuL;
    public GameObject HandObjekBaju;
    public GameObject ListSpriteLakiBaju;
    public GameObject ListSpritePerempuanBaju;


    [Header("Level 3 - Progress 8")]
    public GameObject KarakterLakiAnimSisir;
    public GameObject KarakterPerempuanAnimSisir;
    public GameObject Sisir;
    public GameObject HandSisir;
    public GameObject HandHelpSisir2;

    // Data Progress
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    // Data Controller
    public static LevelDataController levelData;

    // List data Progress terkait
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();

    // Audio


    void Awake()
    {
        // Inisialisasi level data
        levelData = LevelDataController.I;
        ProgressLevel = levelData.GetProgressDataByLevel(Level);
        MainSession = levelData.GetMainSessionDataByLevel(Level);
    }

    void Update()
    {
        ProgressLevel = levelData.GetProgressDataByLevel(Level);
        MainSession = levelData.GetMainSessionDataByLevel(Level);
    }

    void Start()
    {
        // Mulai animasi tangan pada ManagerGameplay
        ManagerGameplay.GetComponent<LevelHandController2>().StartWithJeda(1); // jeda 1 detik

        // Jika tidak ada is_main pada progress
        if (ProgressLevel.All(p => !p.Get_is_main()))
        {
            Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            Debug.LogWarning("Membuat progress 7 menjadi is_main (default)");
            levelData.SetIsMainLevel(Level);
        }

        // Start Audio sesuai dengan ProgressLevel yang is main
        int index = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (index == 0)
        {
            ManagerAudio.instance.PlayVALevel3(1, 1);
        }
        else if (index == 1)
        {
            ManagerAudio.instance.PlayVALevel3(2, 0);
        }

        // Update koin player di UI
        if (AmountKoinPlayer != null)
        {
            int currentKoin = levelData.GetKoinPlayer();
            AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text = currentKoin.ToString();
        }
        else
        {
            Debug.LogWarning("AmountKoinPlayer belum di-assign di inspector.");
        }
    }

    public void OnClickObjek(int nomorGameplay, string jenisKelamin, string namaProgress)
    {
        if (nomorGameplay == 7)
        {
            // Cek Jenis kelamin, apakah sesuai pada LevelData
            if (levelData.GetJenisKelamin() == jenisKelamin)
            {
                HandleGameplayClick(
                    ListGrupGameplayObject[0], BackgroundOpsional,
                    new List<GameObject> { HandKenakanBajuL, HandKenakanBajuP }, 1, nomorGameplay);

                // Start Data Main Session
                levelData.StartLevel_OnClick(Level, namaProgress);

                // Play VA
                ManagerAudio.instance.PlayVALevel3(1, 2);

                // Matikan error objek
                SetOnClickErrorObjek(false);
            }
            else
            {
                Debug.LogWarning("Jenis kelamin tidak sesuai.");
            }

            
        }
        else if (nomorGameplay == 8)
        {
            // Play VA
            ManagerAudio.instance.PlayVALevel3(2, 0);

            // Ubah Background ke Background opsinal 2
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional2.SetActive(true);

            // Ambil fungsi dari GrupObjekGameplay
            ListGrupGameplayObject[1].GetComponent<CekJenisKelaminObject>().CheckGender();

            // Buat database
            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Aktifkan Sisir
            if (Sisir != null)
            {
                Sisir.SetActive(true);
            }

            // Aktifkan HandSisir
            if (HandSisir != null && HandHelpSisir2 != null)
            {
                // stop semua animasi
                StopAllCoroutines();

                // Aktifkan semua hand
                HandSisir.SetActive(true);
                HandHelpSisir2.SetActive(true);

                // Mainkan animasi tangan untuk HandSisir dan HandHelpSisir2
                StartCoroutine(PlayHandAnimationLoopTwoObjects(HandHelpSisir2, HandSisir, 1, 2, 1));
            }

            // Nonaktifkan parent Karakter
            ParentKarakter.SetActive(false);

            // Nonaktifkan Parent Objek
            ParentObjekGameplay.SetActive(false);

            // Matikan error objek
            SetOnClickErrorObjek(false);
        }
    }

    private void HandleGameplayClick(GameObject obj, GameObject background, List<GameObject> HandObjek, int progressIndex, int nomorGameplay)
    {
        // Ubah Background
        background.SetActive(true);

        // Ambil fungsi dari obj Gameplay (hanya aktifkan objek sesuai gender)
        obj.GetComponent<CekJenisKelaminObject>().CheckGender();

        // Aktifkan Hand Objek kalau ada
        if (HandObjek != null)
        {
            // Mainkan animasi tangan
            StopAllCoroutines();

            // Cek Jenis Kelamin
            if (levelData.GetJenisKelamin() == "laki-laki")
                StartCoroutine(PlayHandAnimationLoop(4, 2, HandObjek[0]));
            else if (levelData.GetJenisKelamin() == "perempuan")
                StartCoroutine(PlayHandAnimationLoop(4, 2, HandObjek[1]));
        }

        // Nonaktifkan Parent Karakter
        ParentKarakter.SetActive(false);

        // Nonaktifkan Parent Objek
        ParentObjekGameplay.SetActive(false);

    }

    // OnProgress -> fungsi untuk lanjutan gameplay yang belum selesai
    public void OnProgress(int nomorGameplay, int nomorLevel)
    {
        if (nomorGameplay == 7 && nomorLevel == Level)
        {
            // Implementasikan logika untuk melanjutkan gameplay
            // Nonaktifkan HandHelpKenakanBajuL
            HandKenakanBajuL.SetActive(false);
            // Nonaktifkan HandHelpKenakanBajuP
            HandKenakanBajuP.SetActive(false);

            // Aktifkan HandHelpBaju
            HandObjekBaju.SetActive(true);
            // Stop Animasi sebelumnya
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop(4, 2, HandObjekBaju));

            // Matikan Karakter
            KarakterLakiBaju.SetActive(false);
            KarakterPerempuanBaju.SetActive(false);

            // Aktifkan Sprite list sesuai dengan jenis kelamin
            if (levelData.GetJenisKelamin() == "laki-laki")
            {
                ListSpriteLakiBaju.SetActive(true);
            }
            else if (levelData.GetJenisKelamin() == "perempuan")
            {
                ListSpritePerempuanBaju.SetActive(true);
            }

            // Play Audio
            ManagerAudio.instance.PlayVALevel3(1, 3);
        }
    }

    // OnGameplaySelesai
    public void OnSelesaiGameplay(int nomorGameplay)
    {
        if (nomorGameplay == 7)
        {
            // Implementasikan logika untuk menyelesaikan gameplay
            // Stop animasi tangan
            StopAllCoroutines();

            // Tampilkan window winner
            ShowWindowWinner();

            // Play audio
            ManagerAudio.instance.PlaySFXWinProgress();
            ManagerAudio.instance.PlayVAAfirmasiPositif();

            // Hadiah koin untuk progress ini
            int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[0].id_progress);
            UpdateKoinPlayer(hadiahKoin);

            int CountMain = MainSession.Count;

            // Update status progress main → selesai
            if (CountMain > 0)
            {
               levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[0].id_progress, MainSession[CountMain - 1].id_main, 1);
            }

            // Ubah Koin
            AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

            // Update waktu selesai progress
            levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[0].id_progress);
        }
        else if (nomorGameplay == 8)
        {
            // Implementasikan logika untuk menyelesaikan gameplay
            // Stop animasi tangan
            StopAllCoroutines();

            // Tampilkan window winner
            ShowWindowWinner();

            // Play audio
            ManagerAudio.instance.PlaySFXWinProgress();
            ManagerAudio.instance.PlayVAAfirmasiPositif();

            // Hadiah koin untuk progress ini
            int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[1].id_progress);
            UpdateKoinPlayer(hadiahKoin);
            AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

            int CountMain = MainSession.Count;

            // Update status progress main → selesai
            if (CountMain > 0)
            {
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[1].id_progress, MainSession[CountMain - 1].id_main, 1);
            }

            // Update waktu selesai progress
            levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[1].id_progress);

            // Update data complete level
            string playerId = levelData.GetPlayerID();
            bool levelComplete = levelData.IsLevelCompleted(playerId, nomorGameplay);

            if (levelComplete)
                Debug.Log("Catatan penyelesaian level telah dibuat untuk player ID: " + playerId);
            else
                Debug.Log("Player ID: " + playerId + " data penyelesaian tidak tercatat, eror.");

            Debug.Log($"Gameplay Objek {nomorGameplay} selesai, lanjut ke {(nomorGameplay == 3 ? "Next Progress" : $"Objek {nomorGameplay + 1}")}.");
        }
    }

    /// ---------- Helper ----------
    private void ShowWindowComplete()
    {
        if (WindowComplateLevel != null) WindowComplateLevel.SetActive(true);
        WindowWinner.GetComponent<AnimatorScale>().PlayHide();
        if (ShadowBackground != null) ShadowBackground.SetActive(true);
        if (WindowComplateLevel.GetComponent<AnimatorScale>() != null
           && WindowComplateLevel.activeSelf)
            WindowComplateLevel.GetComponent<AnimatorScale>().PlayShow();
    }

    private void ShowWindowWinner()
    {
        if (ShadowBackground != null) ShadowBackground.SetActive(true);
        if (WindowWinner != null)
        {
            WindowWinner.SetActive(true);
            WindowWinner.GetComponent<AnimatorScale>().PlayShow();
        }
    }

    private void HideWindowWinner()
    {
        if (WindowWinner != null) WindowWinner.GetComponent<AnimatorScale>().PlayHide();
        if (ShadowBackground != null) ShadowBackground.SetActive(false);
    }

    private IEnumerator PlayHandAnimationLoop(int jedaAnimasi, int jedaFirst, GameObject handObject)
    {
        var anim = handObject.GetComponent<IHandAnim>();
        if (anim == null)
        {
            Debug.LogWarning("Tidak ada script animasi yang sesuai di " + handObject.name);
            yield break;
        }

        // pastikan invisible sebelum mulai
        var sr = handObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var c = sr.color; c.a = 0f; sr.color = c;
        }

        // jeda pertama
        if (jedaFirst > 0) yield return new WaitForSeconds(jedaFirst);

        // loop animasi
        while (true)
        {
            handObject.SetActive(true);
            if (anim != null)
            {
                anim.PlayAnimation(); // <-- jalankan animasi swipe
            }

            yield return new WaitForSeconds(jedaAnimasi);
        }
    }
    private IEnumerator PlayHandAnimationLoopTwoObjects(GameObject hand1, GameObject hand2, int delayHand1, int delayHand2, int initialWait = 1)
    {
        if (hand1 == null || hand2 == null) yield break;

        var anim1 = hand1.GetComponent<IHandAnim>();
        var anim2 = hand2.GetComponent<IHandAnim>();

        var sr1 = hand1.GetComponent<SpriteRenderer>();
        var sr2 = hand2.GetComponent<SpriteRenderer>();

        if (sr1 != null) { var c = sr1.color; c.a = 0f; sr1.color = c; }
        if (sr2 != null) { var c = sr2.color; c.a = 0f; sr2.color = c; }

        if (initialWait > 0) yield return new WaitForSeconds(initialWait);

        while (true)
        {
            // Animasi hand1
            hand1.SetActive(true);
            anim1?.PlayAnimation();
            yield return new WaitForSeconds(delayHand1);

            // Animasi hand2
            hand2.SetActive(true);
            anim2?.PlayAnimation();
            yield return new WaitForSeconds(delayHand2);
        }
    }

    // Fungsi helper untuk update koin player
    public void UpdateKoinPlayer(int jumlahKoin)
    {
        if (AmountKoinPlayer != null)
        {
            int currentKoin = int.Parse(AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text);
            int newKoin = currentKoin + jumlahKoin;
            AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text = newKoin.ToString();

            // Update ke LevelDataController
            levelData.UpdateKoinPlayer(newKoin);
        }
        else
        {
            Debug.LogWarning("AmountKoinPlayer belum di-assign di inspector.");
        }
    }

    // Next Progress
    public void NextProgress()
    {
        // Implementasikan logika untuk melanjutkan ke progress berikutnya
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (indexAktif < 0) return; // kalau gak ada progress aktif

        // Update is_main ke false
        levelData.SetProgressMainIsMain(ProgressLevel[indexAktif].id_progress, false);

        // Nonaktifkan window winner + shadow + window
        HideWindowWinner();

        if (indexAktif == 0)
        {
            ManagerGameplay.GetComponent<LevelHandController2>().StartWithJeda(2);

            GantiBackgroundOpsional(0);

            // Aktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(true);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(true);

            // Nonaktifkan objek gameplay index 0 dan 1
            ListGrupObjekGameplay[0].SetActive(false);
            ListGrupObjekGameplay[1].SetActive(false);

            // Nonaktifkan sprite
            ListSpriteLakiBaju.SetActive(false);
            ListSpritePerempuanBaju.SetActive(false);

            // Set OnClick Error
            SetOnClickErrorObjek(true);

            // play Audio
            ManagerAudio.instance.PlayVALevel3(2, 0);
        }
        else if (indexAktif == 1)
        {
            // Tampilkan window complete level
            ShowWindowComplete();

            ManagerAudio.instance.PlaySFXWinLevel();
            ManagerAudio.instance.PlayVAAfirmasiPositif_LevelComplete();
        }
    }

    private void GantiBackgroundOpsional(int index)
    {
        if (index == 0 && BackgroundOpsional != null)
        {
            BackgroundCanvas.SetActive(true);
            BackgroundOpsional.SetActive(false);
        }
        else if (index == 1 && BackgroundOpsional2 != null)
        {
            BackgroundCanvas.SetActive(true);
            BackgroundOpsional2.SetActive(false);
        }
    }

    public void NextLevel()
    {
        // kembalikan ke scane pilihan level
        UnityEngine.SceneManagement.SceneManager.LoadScene(ScanePilihanLevel);
    }

    // Fungsi untuk restart level
    public void UlangiLevel()
    {
        // Set is_main untuk progess utama (indeks 0) menjadi true, dan sisanya false
        for (int i = 0; i < ProgressLevel.Count; i++)
        {
            if (i == 0)
            {
                levelData.SetProgressMain(ProgressLevel[i].id_progress, true);
            }
            else
            {
                levelData.SetProgressMain(ProgressLevel[i].id_progress, false);
            }
        }
        // Ulangi scane saat ini
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        // Debug
        Debug.Log("Tombol Ulangi Progress diklik, ulangi level saat ini.");
    }
    
     // Fungsi untuk setOnClickErrorObjek aktif atau tidak
    public void SetOnClickErrorObjek(bool isActive)
    {
        if (OnClickErrorObjek != null)
        {
            OnClickErrorObjek.SetActive(isActive);
        }
        else
        {
            Debug.LogWarning("OnClickErrorObjek belum di-assign di inspector.");
        }
    }
    


#if UNITY_EDITOR
    [ContextMenu("Load Data")]
    public void LoadDataLevelEditor()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            db = JsonUtility.FromJson<DbRoot>(json);
            Debug.Log("Data level berhasil di-load di editor.");

            string playerName = db.player[0].nama_player;
            string levelId = $"level_{Level}_{playerName}";

            if (db.progress != null)
            {
                ProgressLevel = db.progress
                                  .Where(x => x.fk_id_level == levelId)
                                  .ToList();

                Debug.Log($"[EDITOR] Progress untuk level {Level} ({levelId}) berhasil di-load. Total: {ProgressLevel.Count}");
            }
            else
            {
                ProgressLevel = new List<Progress>();
                Debug.LogWarning("[EDITOR] Database progress kosong atau null.");
            }

            if (db.progress_main != null && ProgressLevel.Count > 0)
            {
                var progressIds = ProgressLevel.Select(p => p.id_progress).ToHashSet();

                ProgressMain = db.progress_main
                                .Where(x => progressIds.Contains(x.fk_id_progress))
                                .ToList();

                Debug.Log($"[EDITOR] ProgressMain untuk semua progress berhasil di-load. Total: {ProgressMain.Count}");
            }
            else
            {
                ProgressMain = new List<ProgressMain>();
                Debug.LogWarning("[EDITOR] ProgressMain tidak ditemukan");
            }

            if (db.waktu_bermain != null && ProgressMain.Count > 0)
            {
                var progressIds = ProgressMain.Select(p => p.fk_id_main).ToHashSet();

                MainSession = db.waktu_bermain
                                .Where(x => progressIds.Contains(x.id_main))
                                .ToList();

                Debug.Log($"[EDITOR] MainSession untuk semua progress berhasil di-load. Total: {MainSession.Count}");
            }
            else 
            {
                MainSession = new List<MainSession>();
                Debug.LogWarning("[EDITOR] Waktu bermain tidak ditemukan");
            }
        }
        else
        {
            Debug.LogWarning("[EDITOR] File data level belum ada di: " + FilePath);
        }
    }
#endif
}
