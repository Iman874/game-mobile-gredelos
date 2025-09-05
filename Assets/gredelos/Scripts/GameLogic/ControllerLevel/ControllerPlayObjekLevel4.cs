using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;


public class ControllerPlayObjekLevel4 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "MenuLevel";

    [Header("Gameplay Manager")]
    public int Level = 4;
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
    public List<GameObject> ListBeforeGrupGameplayObject; // list objek sebelum gameplay
    public List<GameObject> ListAfterGrupGameplayObject; // list objek setelah gameplay
    public GameObject HandObjekMainan;
    public GameObject HandObjekSapu;

    [Header("Level 4 - Progress 9")]
    public GameObject KotakMainan;
    public List<GameObject> ListMainan;
    public GameObject HandHelp4;

    [Header("Level 4 - Progress 10")]
    public GameObject Sapu;
    public GameObject Kemoceng;
    public GameObject HandKemoceng;
    public List<GameObject> ListDebu;
    public GameObject HandHelp5Sapu;
    public GameObject HandHelp5Kemoceng;

    // Data Progress
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data_mod.json");

    // Data Controller
    static LevelDataController levelData;

    [Header("List data Progress terkait")]
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();

    // Variabel lokal
    private int CountListMainan = 0;
    // Animator untuk tangan (khusus hand kemoceng)
    private HandAnimator handAnimator;

    private void Awake()
    {
        db = new DbRoot();
        LoadGameData();
        InitializeGameData();

        // Inisiasi Hand kemoceng
        handAnimator = gameObject.AddComponent<HandAnimator>();
    }

    void Update()
    {
        ProgressLevel = levelData.GetProgressDataByLevel(Level);
        MainSession = levelData.GetMainSessionDataByLevel(Level);
    }

    void Start()
    {
        // Mulai Animasi Tangan
        ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(1); // jeda 1 detik

        // Mulai perhitungan list mainan
        CountListMainan = ListMainan.Count(mainan => mainan.activeInHierarchy);

        // Start Audio sesuai dengan ProgressLevel yang is main
        int index = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (index == 0)
        {
            ManagerAudio.instance.PlayVALevel4(1, 1);
        }
        else if (index == 1)
        {
            ManagerAudio.instance.PlayVALevel4(2, 1);
        }

        // Jika tidak ada is_main pada progress
        if (ProgressLevel.All(p => !p.Get_is_main()))
        {
            Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            Debug.LogWarning("Membuat progress 9 menjadi is_main (default)");
            levelData.SetIsMainLevel(Level);
        }
    }

    public void OnClickObjek(int nomorGameplay, string namaProgress)
    {
        if (nomorGameplay == 9)
        {
            Debug.Log("Progress 9 diaktifkan");
            // Lakukan sesuatu untuk progress 9
            // Stop semua animasi hand
            StopAllCoroutines();

            // Nonaktifkan Hand Objek
            HandObjekMainan.SetActive(false);
            HandObjekSapu.SetActive(false);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Aktifkan HandHelp4
            HandHelp4.SetActive(true);

            // AKtifkan suara
            ManagerAudio.instance.PlayVALevel4(1, 2);

            // Nonaktifkan script OnClick pada objek Kotak mainan
            KotakMainan.GetComponent<OnClickObjek>().enabled = false;

            StartCoroutine(PlayHandAnimationLoop(4, 2, HandHelp4));

            // Nonaktifkan OnclikError
            SetOnClickErrorObjek(false);
        }
        else if (nomorGameplay == 10 && namaProgress != "lanjutan")
        {
            Debug.Log("Progress 10 diaktifkan");
            // Lakukan sesuatu untuk progress 10

            // Stop semua animasi hand
            StopAllCoroutines();

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Nonaktifkan Hand Objek
            HandObjekMainan.SetActive(false);
            HandObjekSapu.SetActive(false);

            // Aktifkan HandHelp5Sapu
            HandHelp5Sapu.SetActive(true);

            // AKtifkan suara
            ManagerAudio.instance.PlayVALevel4(2, 2);

            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelp5Sapu));

            // Nonaktifkan component Onclik pada objek sapu
            Sapu.GetComponent<OnClickObjek>().enabled = false;

            // Aktifkan component DragAndClean
            Sapu.GetComponent<DragAndClean>().enabled = true;

            // Nonaktifkan OnClickError
            SetOnClickErrorObjek(false);
        }
        else if (nomorGameplay == 101 && namaProgress == "lanjutan")
        {
            Debug.Log("Progress 10 tahap 2 diaktifkan");

            // Aktifkan script componen pada kemoceng
            Kemoceng.GetComponent<DragAndClean>().enabled = true;

            // Nonaktifkan Hand kemoceng
            HandKemoceng.SetActive(false);

            // Nonaktifkan component OnClick pada objek kemoceng
            Kemoceng.GetComponent<OnClickObjek>().enabled = false;

            // Aktifkan Hand
            HandHelp5Kemoceng.SetActive(true);

            // Matikan semua animasi
            StopAllCoroutines();

            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelp5Kemoceng));

            // Play sound
            ManagerAudio.instance.PlaySFXClick();
            ManagerAudio.instance.PlayVALevel4(2, 4);

            // Nonaktifkan OnClickError
            SetOnClickErrorObjek(false);
        }
    }
    public void OnProgress(int nomorGameplay, int nomorLevel, string nameDragOpsional)
    {
        if (nomorLevel == 4 && nomorGameplay == 9)
        {
            // Nonaktifkan List Mainan berdasarkan name
            foreach (var mainan in ListMainan)
            {
                if (mainan.name == nameDragOpsional)
                {
                    mainan.SetActive(false);
                }
            }

            // Selalu kurangi -1 pada CountListMainan
            CountListMainan--;

            // Jika CountListMainan <= 0, berarti semua mainan sudah dinonaktifkan
            if (CountListMainan <= 0)
            {
                Debug.Log("Semua mainan sudah dinonaktifkan");

                // Aktifkan OnClickError
                SetOnClickErrorObjek(true);

                // Progress 9 selesai panggil OnSelesaiGameplay
                OnSelesaiGameplay(nomorGameplay);
            }

        }
        else if (nomorLevel == 4 && nomorGameplay == 10)
        {
            Debug.Log("Progress 10 tahap 2 diaktifkan");

            // Buat agar sapu ke posisi semula
            Sapu.GetComponent<DragAndClean>().EndDrag();

            // Stop semua Animasi
            StopAllCoroutines();

            // Nonaktifkan script pada sapu
            Sapu.GetComponent<OnClickObjek>().enabled = false;
            Sapu.GetComponent<DragAndClean>().enabled = false;

            // Aktifkan component pada kemoceng
            Kemoceng.GetComponent<OnClickObjek>().enabled = true;

            // Aktifkan OnClickError
            SetOnClickErrorObjek(true);

            // Aktifkan Hand Kemoceng
            HandKemoceng.SetActive(true);

            // Putar animasi HandKemoceng
            // --- Putar animasi HandKemoceng ---
            handAnimator.durasiAnimasi = 3f;   // bisa diubah sesuai kebutuhan
            handAnimator.jedaAnimasi = 4f;     // bisa diubah sesuai kebutuhan
            handAnimator.PlayAnimationLoop(HandKemoceng);

            ManagerAudio.instance.PlayVALevel4(2, 3);
        }
        else if (nomorLevel == 4 && nomorGameplay == 101)
        {
            // Nonaktifkan script componen pada kemoceng
            Kemoceng.GetComponent<DragAndClean>().enabled = false;

            // Play sound
            ManagerAudio.instance.PlaySFXClick();
            ManagerAudio.instance.PlayVALevel4(2, 4);

            // On Selesai Gameplay
            OnSelesaiGameplay(10);
        }

    }

    public void OnSelesaiGameplay(int nomorGameplay)
    {
        Debug.Log("Gameplay selesai untuk nomor: " + nomorGameplay);

        if (nomorGameplay == 9)
        {
            // Stop semua animasi tangan
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
        else if (nomorGameplay == 10)
        {
            // Implementasikan logika untuk progress 10
            // Gameplay telah selesai
            // Stop semua animasi tangan
            StopAllCoroutines();

            // Tampilkan window winner
            ShowWindowWinner();

            // Play audio
            ManagerAudio.instance.PlaySFXWinProgress();
            ManagerAudio.instance.PlayVAAfirmasiPositif();

            // Hadiah koin untuk progress ini
            int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[1].id_progress);
            UpdateKoinPlayer(hadiahKoin);

            int CountMain = MainSession.Count;

            // Update status progress main → selesai
            if (CountMain > 0)
            {
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[1].id_progress, MainSession[CountMain - 1].id_main, 1);
            }

            // Ubah Koin
            AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

            // Update waktu selesai progress
            levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[1].id_progress);

            // Update data complete level
            string playerId = levelData.GetPlayerID();
            bool levelComplete = levelData.IsLevelCompleted(playerId, nomorGameplay);

            if (levelComplete)
                Debug.Log("Catatan penyelesaian level telah dibuat untuk player ID: " + playerId);
            else
                Debug.Log("Player ID: " + playerId + " data penyelesaian tidak tercatat, eror.");

            Debug.Log($"Gameplay Objek {nomorGameplay} selesai.");
        }
    }

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
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(2);

            // Play Audio
            ManagerAudio.instance.PlayVALevel4(2, 1);

            // Aktifkan OnClickError
            SetOnClickErrorObjek(true);
        }
        else if (indexAktif == 1)
        {
            ShowWindowComplete();

            // Play Audio
            ManagerAudio.instance.PlaySFXWinLevel();
            ManagerAudio.instance.PlayVAAfirmasiPositif_LevelComplete();
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

    private void LoadGameData()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            db = JsonUtility.FromJson<DbRoot>(json);
        }
    }

    private void InitializeGameData()
    {
        // Inisialisasi level data
        levelData = LevelDataController.I;
        ProgressLevel = levelData.GetProgressDataByLevel(Level);
        MainSession = levelData.GetMainSessionDataByLevel(Level);
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
        if (jedaFirst > 0)
            yield return new WaitForSeconds(jedaFirst);

        var handHelp = handObject.GetComponent<HandHelp4>();

        while (handObject.activeInHierarchy)
        {
            // aktifkan handObject
            handObject.SetActive(true);

            // jalankan animasi satu putaran penuh
            anim.PlayAnimation();

            // tunggu sampai animasi selesai (semua destinasi selesai)
            while (handHelp != null && handHelp.IsPlaying)
            {
                yield return null;
            }

            // tunggu jeda sebelum mengulang animasi dari awal
            if (jedaAnimasi > 0)
                yield return new WaitForSeconds(jedaAnimasi);
        }
    }

    private IEnumerator PlayHandAnimationLoop2(int jedaAnimasi, int jedaFirst, GameObject handObject)
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
        if (jedaFirst > 0)
            yield return new WaitForSeconds(jedaFirst);

        // loop animasi
        while (handObject.activeInHierarchy)
        {
            // aktifkan handObject
            handObject.SetActive(true);

            // jalankan animasi satu putaran penuh
            anim.PlayAnimation();

            // tunggu sampai animasi selesai (menggunakan property IsPlaying)
            while (anim.IsPlaying)
            {
                yield return null;
            }

            // tunggu jeda sebelum mengulang animasi dari awal
            if (jedaAnimasi > 0)
                yield return new WaitForSeconds(jedaAnimasi);
        }
    }


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

    private void ShowWindowComplete()
    {
        if (WindowComplateLevel != null) WindowComplateLevel.SetActive(true);
        WindowWinner.GetComponent<AnimatorScale>().PlayHide();
        if (ShadowBackground != null) ShadowBackground.SetActive(true);
        if (WindowComplateLevel.GetComponent<AnimatorScale>() != null
           && WindowComplateLevel.activeSelf)
            WindowComplateLevel.GetComponent<AnimatorScale>().PlayShow();
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