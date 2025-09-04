using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

public class ControllerPlayObjekLevel1 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "PilihanLevel";

    [Header("Gameplay Manager")]
    public int Level = 1;
    public GameObject ParentObjekGameplay;
    public GameObject ManagerGameplay; // manager gameplay

    [Header("List Objek Gameplay")]
    public List<GameObject> ListGrupObjekGameplay; // list objek gameplay

    [Header("List Objek After Gameplay")]
    public List<GameObject> ListObjekAfterGameplay; // list objek after gameplay

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

    [Header("Objek Gameplay")]
    public GameObject BackgroundCanvas; // Background canvas
    public GameObject BackgroundOpsional; // Background gameplay
    public GameObject BackgroundOpsional2; // Background gameplay 2
    public GameObject ParentKarakter; // Background default
    public bool ChangeBackground;
    public GameObject GameplayObjek1;
    // Hand Help
    public GameObject hand1;
    public GameObject GameplayObjek2;
    // Hand Help
    public GameObject hand2;
    public GameObject GameplayObjek3;
    // Hand Help
    public GameObject hand3;
    public static LevelDataController levelData;
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();
    private List<Coroutine> runningCoroutines = new();

    // Data Progress
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    void Awake()
    {
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
        ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(1); // jeda 5 detik

        // Jika tidak ada is_main pada progress
        if (ProgressLevel.All(p => !p.Get_is_main()))
        {
            Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            Debug.LogWarning("Membuat progress 1 menjadi is_main (default)");
            levelData.SetIsMainLevel(Level);
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

        UpdateUI(); // panggil update sekali di start untuk inisialisasi state awal
    }

   

    // Fungsi untuk update VA
    public void CekIsMainProgress()
    {
        if (ProgressLevel.Count > 0)
        {
            int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
            if (indexAktif == -1)
            {
                Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            }
            else if (indexAktif == 0)
            {
                Debug.Log("Progress utama adalah progress " + (indexAktif + 1));
                // Putar VA sesuai progress utama
                ManagerAudio.instance.PlayVALevel1Progress1();
            }
            else if (indexAktif == 1)
            {
                Debug.Log("Progress utama adalah progress 2");
                // Putar VA sesuai progress utama
                ManagerAudio.instance.PlayVALevel1Progress2();
            }
            else if (indexAktif == 2)
            {
                Debug.Log("Progress utama adalah progress 3");
                // Putar VA sesuai progress utama
                ManagerAudio.instance.PlayVALevel1Progress3();
            }
            else
            {
                Debug.LogWarning("ProgressLevel kosong di level " + Level);
            }
        }
    }

    // Selalu Update state is main pada progeress
    public void UpdateUI()
    {
        if (ProgressLevel.Count > 0)
        {
            int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
            if (indexAktif == -1)
            {
                Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            }

            // Nonaktifkan objek pada list objek gameplay indeks yang telah lewat
            if (indexAktif == 0)
            {
                Debug.Log("Progress utama adalah progress 1");
            }
            else if (indexAktif == 1)
            {
                Debug.Log("Progress utama adalah progress 2");
                // Nonaktifkan objek pada list objek gameplay pada indeks 0
                if (ListGrupObjekGameplay.Count > 0 && ListGrupObjekGameplay[0] != null)
                {
                    ListGrupObjekGameplay[0].SetActive(false);
                    // Aktifkan objek pada list objek after gameplay pada indeks 0
                    if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
                    {
                        ListObjekAfterGameplay[0].SetActive(true);
                    }
                }
            }
            else if (indexAktif == 2)
            {
                Debug.Log("Progress utama adalah progress 3");
                // Nonaktifkan objek pada list objek gameplay pada indeks 0 dan 1
                if (ListGrupObjekGameplay.Count > 1 && ListGrupObjekGameplay[1] != null && 
                   ListGrupObjekGameplay[0] != null)
                {
                    ListGrupObjekGameplay[0].SetActive(false);
                    ListGrupObjekGameplay[1].SetActive(false);
                    // Aktifkan objek pada list objek after gameplay pada indeks 1
                    if (ListObjekAfterGameplay.Count > 1 && ListObjekAfterGameplay[1] != null
                    && ListObjekAfterGameplay[0] != null)
                    {
                        ListObjekAfterGameplay[0].SetActive(true);
                        ListObjekAfterGameplay[1].SetActive(true);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("ProgressLevel kosong di level " + Level);
        }
    }

    public void OnClickObjek(int nomorGameplay, string namaProgress)
    {
        if (nomorGameplay == 1)
        {
            HandleGameplayClick(GameplayObjek1, BackgroundOpsional, null, 1, nomorGameplay);

            // Buat database
            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);
        }
        else if (nomorGameplay == 2)
        {
            HandleGameplayClick(GameplayObjek2, BackgroundOpsional2,
                new List<GameObject>{
                    ListObjekAfterGameplay.Count > 0 ? ListObjekAfterGameplay[0] : null
                }, 2, nomorGameplay);

            // Buat database
            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

        }
        else if (nomorGameplay == 3)
        {
            HandleGameplayClick(GameplayObjek3, null,
                new List<GameObject> {
                    ListObjekAfterGameplay.Count > 0 ? ListObjekAfterGameplay[0] : null,
                    ListObjekAfterGameplay.Count > 1 ? ListObjekAfterGameplay[1] : null
                }, 3, nomorGameplay);

            // Buat database
            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);
        }
    }

    // Helper
    private void HandleGameplayClick(
        GameObject gameplayObjek,
        GameObject backgroundOpsional,
        List<GameObject> objekAfterGameplay,
        int handAnimIndex, int nomorGameplay)
    {
        // Nonaktifkan karakter
        if (ParentKarakter != null && nomorGameplay != 3)
            ParentKarakter.SetActive(false);

        // Nonaktifkan parent objek gameplay
        if (ParentObjekGameplay != null)
            ParentObjekGameplay.SetActive(false);

        // Ubah background kalau ada
        if (ChangeBackground && backgroundOpsional != null)
        {
            BackgroundCanvas.SetActive(false);
            backgroundOpsional.SetActive(true);
        }

        // Nonaktifkan objek after gameplay kalau ada
        if (objekAfterGameplay != null)
        {
            foreach (var obj in objekAfterGameplay)
            {
                if (obj != null && nomorGameplay == 2)
                {
                    obj.SetActive(false);
                }
                else if (obj != null)
                {
                  obj.SetActive(true);  
                } 
            }
        }

        // Panggil VA
        CekIsMainProgress();

        // Aktifkan objek gameplay
        if (gameplayObjek != null)
            gameplayObjek.SetActive(true);

        // Stop semua coroutine lama biar gak dobel
        StopAllCoroutines();
        StartCoroutine(PlayHandAnimationLoop(4, 2, handAnimIndex));

        // Log
        Debug.Log($"Gameplay {handAnimIndex} aktif dan animasi tangan dimulai.");

        // Matikan error objek
        SetOnClickErrorObjek(false);
    }


    // Fungsi Setelah Progress Gameplay Selesai
    public void OnSelesaiGameplay(int nomorGameplay)
    {
        int index = nomorGameplay - 1; // biar gampang akses list (0-based)

        if (index < 0 || index >= ProgressLevel.Count) return;

        // Stop animasi tangan
        StopAllCoroutines();

        // Tampilkan window winner
        ShowWindowWinner();

        // Play audio
        ManagerAudio.instance.PlaySFXWinProgress();
        ManagerAudio.instance.PlayVAAfirmasiPositif();

        // Hadiah koin untuk progress ini
        int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[index].id_progress);
        UpdateKoinPlayer(hadiahKoin);

        int CountMain = MainSession.Count;

        if (CountMain > 0)
        {
            // Update status progress main → selesai
            levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[index].id_progress, MainSession[CountMain - 1].id_main, 1);
            AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

            Debug.Log($"Main Session {CountMain} selesai, hadiah koin: {hadiahKoin}");
        }

        // Update waktu selesai progress
        levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[index].id_progress);

        // Khusus progress terakhir (misalnya progress ke-3)
        if (nomorGameplay == 3)
        {
            string playerId = levelData.GetPlayerID();
            bool levelComplate = levelData.IsLevelCompleted(playerId, nomorGameplay);

            if (levelComplate)
                Debug.Log("Catatan penyelesaian level telah dibuat untuk player ID: " + playerId);
            else
                Debug.Log("Player ID: " + playerId + " data penyelesaian tidak tercatat, eror.");
        }

        Debug.Log($"Gameplay Objek {nomorGameplay} selesai, lanjut ke {(nomorGameplay == 3 ? "Next Progress" : $"Objek {nomorGameplay + 1}")}.");
    }

    /// ---------------- Helper ----------------
    private void ShowWindowWinner()
    {
        if (ShadowBackground != null) ShadowBackground.SetActive(true);
        if (WindowWinner != null)
        {
            WindowWinner.SetActive(true);
            WindowWinner.GetComponent<AnimatorScale>().PlayShow();
        }
    }


    public void NextProgress()
    {
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (indexAktif < 0) return; // kalau gak ada progress aktif

        // Matikan objek gameplay sesuai index
        MatikanGameplayObjek(indexAktif);

        // Update is_main ke false
        levelData.SetProgressMainIsMain(ProgressLevel[indexAktif].id_progress, false);

        // Nonaktifkan window winner + shadow + window
        HideWindowWinner();

        // Progress 1 dan 2 → lanjut ke progress berikutnya
        if (indexAktif == 0 || indexAktif == 1)
        {
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(5);
            CekIsMainProgress();

            // Ganti background opsional sesuai progress
            GantiBackgroundOpsional(indexAktif);

            // Aktifkan karakter & objek gameplay
            if (ParentKarakter != null) ParentKarakter.SetActive(true);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(true);

            // Nonaktifkan objek gameplay pada index
            if (ListGrupObjekGameplay.Count > indexAktif && ListGrupObjekGameplay[indexAktif] != null)
                ListGrupObjekGameplay[indexAktif].SetActive(false);

            // Aktifkan objek after gameplay pada index 0
            if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
                ListObjekAfterGameplay[0].SetActive(true);

            // Khusus progress ke-2: aktifkan juga after gameplay 1
            if (indexAktif == 1 && ListObjekAfterGameplay.Count > 1 && ListObjekAfterGameplay[0] != null
                && ListObjekAfterGameplay[1] != null)
            {
                ListObjekAfterGameplay[0].SetActive(true);
                ListObjekAfterGameplay[1].SetActive(true);
            }

            SetOnClickErrorObjek(true);
        }
        else if (indexAktif == 2) // Progress terakhir
        {
            // Selesai semua progress
            if (WindowComplateLevel != null)
            {
                ShowWindowComplete();
            }

            ManagerAudio.instance.PlaySFXWinLevel();
            ManagerAudio.instance.PlayVAAfirmasiPositif_LevelComplete();

            if (ListGrupObjekGameplay.Count > 2 && ListGrupObjekGameplay[2] != null)
                ListGrupObjekGameplay[2].SetActive(false);

            if (ListObjekAfterGameplay.Count > 2 && ListObjekAfterGameplay[2] != null)
                ListObjekAfterGameplay[2].SetActive(true);

            Debug.Log("Semua progress di level ini telah diselesaikan.");
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

    private void MatikanGameplayObjek(int index)
    {
        if (index == 0 && GameplayObjek1 != null) GameplayObjek1.SetActive(false);
        if (index == 1 && GameplayObjek2 != null) GameplayObjek2.SetActive(false);
        if (index == 2 && GameplayObjek3 != null) GameplayObjek3.SetActive(false);
    }

    private void HideWindowWinner()
    {
        if (WindowWinner != null) WindowWinner.GetComponent<AnimatorScale>().PlayHide();
        if (ShadowBackground != null) ShadowBackground.SetActive(false);
    }

    private void GantiBackgroundOpsional(int index)
    {
        if (!ChangeBackground) return;

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

    public void NextLevel()
    {
        // kembalikan ke scane pilihan level
        UnityEngine.SceneManagement.SceneManager.LoadScene(ScanePilihanLevel);

    }

    // Fungsi Helper
    private IEnumerator PlayHandAnimationLoop(int jedaAnimasi, int jedaFirst, int nomorHand)
    {
        GameObject targetHand = null;

        if (nomorHand == 1) targetHand = hand1;
        else if (nomorHand == 2) targetHand = hand2;
        else if (nomorHand == 3) targetHand = hand3;

        if (targetHand == null) yield break; // kalau gak ada hand, langsung stop

        // pastikan tidak terlihat sebelum animasi dimulai
        var sr = targetHand.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        }

        // jeda pertama kali (sekali aja)
        if (jedaFirst > 0)
        {
            yield return new WaitForSeconds(jedaFirst);
        }

        // loop animasi terus
        while (true)
        {
            if (targetHand != null)
            {
                HandHelp anim = targetHand.GetComponent<HandHelp>();
                if (anim != null)
                {
                    anim.PlayAnimation(); // panggil method di script HandHelp
                    Debug.Log($"Animasi tangan {nomorHand} diputar");
                }
            }

            yield return new WaitForSeconds(jedaAnimasi);
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
