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
    }

    void Start()
    {
        // Mulai animasi tangan pada ManagerGameplay
        ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(1); // jeda 5 detik

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
        }
        else
        {
            Debug.LogWarning("ProgressLevel kosong di level " + Level);
        }
    }

    public void OnClickObjek(int nomorGameplay)
    {
        if (nomorGameplay == 1)
        {
            // Logika untuk objek
            ParentObjekGameplay.SetActive(false);
            GameplayObjek1.SetActive(true);

            // Ubah background jika ada
            if (ChangeBackground && BackgroundOpsional != null)
            {
                BackgroundCanvas.SetActive(false);
                BackgroundOpsional.SetActive(true);
            }

            // Nonaktifkan karakter
            if (ParentKarakter != null)
            {
                ParentKarakter.SetActive(false);
            }

            // Panggil VA
            CekIsMainProgress();

            // Stop semua coroutine lama biar gak dobel
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop(4, 4, 1));

            Debug.Log("Objek Gameplay 1 aktif dan animasi tangan dimulai.");
        }
        else if (nomorGameplay == 2)
        {
            // Ganti background jika ada
            if (ChangeBackground && BackgroundOpsional2 != null)
            {
                BackgroundCanvas.SetActive(false);
                BackgroundOpsional2.SetActive(true);
            }

            // Nonaktifkan karakter
            if (ParentKarakter != null)
            {
                ParentKarakter.SetActive(false);
            }

            // Nonaktifkan parent objek gameplay
            if (ParentObjekGameplay != null)
            {
                ParentObjekGameplay.SetActive(false);
            }

            // Nonaktifkan objek after gameplay pada indeks 0
            if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
            {
                ListObjekAfterGameplay[0].SetActive(false);
            }

            // Panggil VA
            CekIsMainProgress();

            GameplayObjek2.SetActive(true);

            // Stop semua coroutine lama biar gak dobel
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop(7, 4, 2));

            // Logika untuk objek kedua
            Debug.Log("Objek kedua diklik: " + gameObject.name);
        }
        else if (nomorGameplay == 3)
        {
            // Logika untuk objek ketiga
            Debug.Log("Objek ketiga diklik: " + gameObject.name);

            // Nonaktfikan parent objek gameplay
            if (ParentObjekGameplay != null)
            {
                ParentObjekGameplay.SetActive(false);
            }

            // Aktifkan objek after gameplay pada indeks 0 dan indeks 1
            if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
            {
                ListObjekAfterGameplay[0].SetActive(true);
            }
            if (ListObjekAfterGameplay.Count > 1 && ListObjekAfterGameplay[1] != null)
            {
                ListObjekAfterGameplay[1].SetActive(true);
            }

            // Panggil VA
            CekIsMainProgress();

            // Aktifkan objek gameplay 3
            GameplayObjek3.SetActive(true);

            // Stop semua coroutine lama biar gak dobel
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop(4, 4, 3));
            Debug.Log("Objek Gameplay 3 aktif dan animasi tangan dimulai.");
        }
    }

    // Fungsi Setelah Progress Gameplay Selesai
    public void OnSelesaiGameplay(int nomorGameplay)
    {
        if (nomorGameplay == 1)
        {
            // Hentikan animasi tangan
            StopAllCoroutines();

            // Aktifkan window
            if (WindowObjekParent != null)
            {
                WindowObjekParent.SetActive(true);

                // Aktifkan shadow background
                if (ShadowBackground != null)
                {
                    ShadowBackground.SetActive(true);
                }

                // Aktifkan window winner
                if (WindowWinner != null)
                {
                    WindowWinner.SetActive(true);
                    WindowWinner.GetComponent<AnimatorScale>().PlayShow();
                }

                ManagerAudio.instance.PlaySFXWinProgress(); // play SFX win progress
                ManagerAudio.instance.PlayVAAfirmasiPositif(); // play VA afirmasi positif

                // Cek hadiah koin dari progress 1
                int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[0].id_progress);
                //if (levelData.GetStatusPenyelesaianProgressMain(ProgressLevel[0].id_progress) == 1)
                //{
                //    hadiahKoin = 10; // jik sudah pernah diselesaikan, hadiah dikurangi
                //}

                // Update koin player
                UpdateKoinPlayer(hadiahKoin);

                // Cek dan update status penyelesaian progress main sudah 1 (sudah diselesaikan)
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[0].id_progress, 1);
                AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

                // Update waktu selesai progress
                levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[0].id_progress);
            }
            Debug.Log("Gampelay Objek 1 selesai, lanjut ke Objek 2 dan animasi tangan dimulai.");
        }

        // Tambahkan logika untuk nomorGameplay 2
        if (nomorGameplay == 2)
        {
            // Hentikan animasi tangan
            StopAllCoroutines();

            // Aktifkan window
            if (WindowObjekParent != null)
            {
                WindowObjekParent.SetActive(true);

                // Aktifkan shadow background
                if (ShadowBackground != null)
                {
                    ShadowBackground.SetActive(true);
                }

                // Aktifkan window winner
                if (WindowWinner != null)
                {
                    WindowWinner.SetActive(true);
                    WindowWinner.GetComponent<AnimatorScale>().PlayShow();
                }

                ManagerAudio.instance.PlaySFXWinProgress(); // play SFX win progress
                ManagerAudio.instance.PlayVAAfirmasiPositif(); // play VA afirmasi positif

                // Cek hadiah koin dari progress 2
                int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[1].id_progress);

                // Update koin player
                UpdateKoinPlayer(hadiahKoin);

                // Cek dan update status penyelesaian progress main sudah 1 (sudah diselesaikan)
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[1].id_progress, 1);
                AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

                // Update waktu selesai progress
                levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[1].id_progress);
            }
            Debug.Log("Gampelay Objek 2 selesai, lanjut ke Objek 3 dan animasi tangan dimulai.");
        }

        // Tambahkan logika untuk nomorGameplay 3
        if (nomorGameplay == 3)
        {
            // Hentikan animasi tangan
            StopAllCoroutines();

            // Aktifkan window
            if (WindowObjekParent != null)
            {
                WindowObjekParent.SetActive(true);

                // Aktifkan shadow background
                if (ShadowBackground != null)
                {
                    ShadowBackground.SetActive(true);
                }

                // Aktifkan window winner
                if (WindowWinner != null)
                {
                    WindowWinner.SetActive(true);
                    WindowWinner.GetComponent<AnimatorScale>().PlayShow();
                }

                ManagerAudio.instance.PlaySFXWinProgress(); // play SFX win progress
                ManagerAudio.instance.PlayVAAfirmasiPositif(); // play VA afirmasi positif

                // Cek hadiah koin dari progress 3
                int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[2].id_progress);

                // Update koin player
                UpdateKoinPlayer(hadiahKoin);

                // Cek dan update status penyelesaian progress main sudah 1 (sudah diselesaikan)
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[2].id_progress, 1);
                AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

                // Update waktu selesai progress
                levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[2].id_progress);
            }
            Debug.Log("Gampelay Objek 3 selesai, lanjut ke Next Progress.");
        }

    }

    // Fungsi untuk tombol lanjut di window
    public void NextProgress()
    {
        // Cek progress level mana yang aktif (is_main == true)
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());

        if (indexAktif == 0) // indeks 0 adalah nomor progress 1
        {
            // Lanjut ke progress 2
            if (GameplayObjek1 != null) GameplayObjek1.SetActive(false);
            // Buat agar progress 1 is_main = false, dan progress 2 is_main = true
            levelData.SetProgressMainIsMain(ProgressLevel[0].id_progress, false);

            // Nonaktifkan window winner dengan animasi
            if (WindowWinner != null)
            {
                WindowWinner.GetComponent<AnimatorScale>().PlayHide();
            }

            // Nonaktifkan shadow background
            if (ShadowBackground != null)
            {
                ShadowBackground.SetActive(false);
            }

            // Nonaktifkan window
            if (WindowObjekParent != null)
            {
                WindowObjekParent.SetActive(false);
            }

            // Jalankan animasi tangan untuk progress 2
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(5); // jeda 5 detik
            CekIsMainProgress(); // cek progress mana yang is_main = true

            // Nonkatifkan background opsional, kembalikan background canvas
            if (ChangeBackground && BackgroundOpsional != null)
            {
                BackgroundCanvas.SetActive(true);
                BackgroundOpsional.SetActive(false);
            }

            // Aktifkan parent karakter
            if (ParentKarakter != null)
            {
                ParentKarakter.SetActive(true);
            }

            // Aktifkan parent objek gameplay
            if (ParentObjekGameplay != null)
            {
                ParentObjekGameplay.SetActive(true);
            }

            // Nonaktifkan objek pada list objek gameplay pada indeks 0
            if (ListGrupObjekGameplay.Count > 0 && ListGrupObjekGameplay[0] != null)
            {
                ListGrupObjekGameplay[0].SetActive(false);
            }

            // Aktifkan objek pada list objek after gameplay pada indeks 0
            if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
            {
                ListObjekAfterGameplay[0].SetActive(true);
            }
        }

        else if (indexAktif == 1) // indeks 1 adalah nomor progress 2
        {
            // Lanjut ke progress 3
            if (GameplayObjek2 != null) GameplayObjek2.SetActive(false);
            // Buat agar progress 2 is_main = false, dan progress 3 is_main = true
            levelData.SetProgressMainIsMain(ProgressLevel[1].id_progress, false);

            // Nonaktifkan window winner dengan animasi
            if (WindowWinner != null)
            {
                WindowWinner.GetComponent<AnimatorScale>().PlayHide();
            }

            // Nonaktifkan shadow background
            if (ShadowBackground != null)
            {
                ShadowBackground.SetActive(false);
            }

            // Nonaktifkan window
            if (WindowObjekParent != null)
            {
                WindowObjekParent.SetActive(false);
            }

            // Jalankan animasi tangan untuk progress 3
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(5); // jeda 5 detik
            CekIsMainProgress(); // cek progress mana yang is_main = true

            // Nonkatifkan background opsional2, kembalikan background canvas
            if (ChangeBackground && BackgroundOpsional2 != null)
            {
                BackgroundCanvas.SetActive(true);
                BackgroundOpsional2.SetActive(false);
            }

            // Aktifkan parent karakter
            if (ParentKarakter != null)
            {
                ParentKarakter.SetActive(true);
            }

            // Aktifkan parent objek gameplay
            if (ParentObjekGameplay != null)
            {
                ParentObjekGameplay.SetActive(true);
            }

            // Nonaktifkan objek pada list objek gameplay pada indeks 1
            if (ListGrupObjekGameplay.Count > 1 && ListGrupObjekGameplay[1] != null)
            {
                ListGrupObjekGameplay[1].SetActive(false);
            }

            // Aktifkan objek pada list objek after gameplay pada indeks 1
            if (ListObjekAfterGameplay.Count > 1 && ListObjekAfterGameplay[1] != null)
            {
                ListObjekAfterGameplay[1].SetActive(true);
            }

            // Aktifkan juga after gameplay 1
            if (ListObjekAfterGameplay.Count > 0 && ListObjekAfterGameplay[0] != null)
            {
                ListObjekAfterGameplay[0].SetActive(true);
            }
        }
        else if (indexAktif == 2) // indeks 2 adalah nomor progress 3
        {
            // Selesai semua progress di level ini
            if (GameplayObjek3 != null) GameplayObjek3.SetActive(false);
            // Buat agar progress 3 is_main = false
            levelData.SetProgressMainIsMain(ProgressLevel[2].id_progress, false);

            // Nonaktifkan window winner dengan animasi
            if (WindowWinner != null)
            {
                WindowWinner.GetComponent<AnimatorScale>().PlayHide();
            }

            // Aktifkan window complate level
            if (WindowComplateLevel != null)
            {
                WindowComplateLevel.SetActive(true);
                WindowComplateLevel.GetComponent<AnimatorScale>().PlayShow();
            }

            ManagerAudio.instance.PlaySFXWinLevel(); // play SFX win level
            ManagerAudio.instance.PlayVAAfirmasiPositif_LevelComplete(); // play VA level complete

            // Nonaktifkan objek pada list objek gameplay pada indeks 3
            if (ListGrupObjekGameplay.Count > 2 && ListGrupObjekGameplay[2] != null)
            {
                ListGrupObjekGameplay[2].SetActive(false);
            }

            // Aktifkan objek pada list objek after gameplay pada indeks 3
            if (ListObjekAfterGameplay.Count > 2 && ListObjekAfterGameplay[2] != null)
            {
                ListObjekAfterGameplay[2].SetActive(true);
            }

            Debug.Log("Semua progress di level ini telah diselesaikan.");
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
        if (nomorHand == 1)
        {
            // pastikan hand1 tidak terlihat sebelum animasi dimulai
            var sr = hand1.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // invisible dulu
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
                if (hand1 != null)
                {
                    HandHelp anim = hand1.GetComponent<HandHelp>();
                    if (anim != null)
                    {
                        // Aktifkan hand1
                        anim.PlayAnimation(); // panggil method di script HandHelp
                        Debug.Log("Animasi tangan diputar");
                    }
                }

                yield return new WaitForSeconds(jedaAnimasi);
            }
        }

        else if (nomorHand == 2)
        {
            // pastikan hand2 tidak terlihat sebelum animasi dimulai
            var sr = hand2.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // invisible dulu
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
                if (hand2 != null)
                {
                    HandHelp anim = hand2.GetComponent<HandHelp>();
                    if (anim != null)
                    {
                        // Aktifkan hand2
                        anim.PlayAnimation(); // panggil method di script HandHelp
                        Debug.Log("Animasi tangan diputar");
                    }
                }

                yield return new WaitForSeconds(jedaAnimasi);
            }
        }

        else if (nomorHand == 3)
        {
            // pastikan hand3 tidak terlihat sebelum animasi dimulai
            var sr = hand3.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // invisible dulu
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
                if (hand3 != null)
                {
                    HandHelp anim = hand3.GetComponent<HandHelp>();
                    if (anim != null)
                    {
                        // Aktifkan hand3
                        anim.PlayAnimation(); // panggil method di script HandHelp
                        Debug.Log("Animasi tangan diputar");
                    }
                }

                yield return new WaitForSeconds(jedaAnimasi);
            }
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
