using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

public class ControllerPlayObjekLevel2 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "MenuLevel";

    [Header("Gameplay Manager")]
    public int Level = 2;
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
    public GameObject BackgroundOpsional3; // Background gameplay

    [Header("Gameplay 2 Progress 1")]
    public GameObject ParentGameplayGosokGigi; // parent objek gameplay 2_1
    public GameObject SikatGigi; // objek sikat gigi
    public GameObject Odol; // objek odol
    public GameObject CairanOdol; // objek tutup odol
    public GameObject Gigi; // objek Gigi
    public GameObject SpriteKarakterGigi_L; // sprite karakter gigi L
    public GameObject SpriteKarakterGigi_P; // sprite karakter gigi P
    public GameObject ParentListNodaGigi; // list noda gigi
    public List<GameObject> ListNodaGigi; // list noda gigi

    [Header("Gameplay 2 Progress 1 Hand Help")]
    public GameObject HandHelpOdol; // objek hand help odol
    public GameObject HandHelpGosokGigi; // objek hand help gosok gigi

    [Header("Gameplay 2 Progress 2")]
    public GameObject ParentGameplayMandi; // parent objek gameplay 2_2
    public GameObject Sabun; // objek sabun
    public GameObject Shampoo; // objek shampoo
    public GameObject ParentGayung; // objek handuk
    public GameObject BakMandi; // Bak mandi
    public GameObject KarakterMandiSabun_L; // objek gayung
    public GameObject KarakterMandiSabun_P; // objek gayung
    public GameObject KarakterMandiShampoo_L; // objek gayung
    public GameObject KarakterMandiShampoo_P; // objek gayung
    public GameObject KarakterMandiGayung_L; // objek gayung
    public GameObject KarakterMandiGayung_P; // objek gayung
    public GameObject EfekBusaSabun; // efek busa sabun
    public GameObject EfekBusaShampoo; // efek busa shampoo

    [Header("Gameplay 2 Progress 2 Hand Help")]
    public GameObject HandHelpSabun; // objek hand help sabun
    public GameObject HandHelpShampoo; // objek hand help shampoo
    public GameObject HandHelpGayung; // objek hand help gayung

    [Header("Gameplay 2 Progress 3")]
    public GameObject ParentGameplayPengering; // parent objek gameplay 2_3
    public GameObject HairDryer; // objek hair dryer
    public GameObject SpriteKarakterPengering_L; // objek karakter pengering L
    public GameObject SpriteKarakterPengering_P; // objek karakter pengering P
    public GameObject ParentEfekAirList; // objek efek air

    [Header("Gameplay 2 Progress 3 Hand Help")]
    public GameObject HandHelpHairDryer; // objek hand help hair dryer

    // Database
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    // Data Controller
    static LevelDataController levelData;

    [Header("List data Progress terkait")]
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();

    // Animator untuk tangan (khusus untuk progress)
    private HandAnimator handAnimator;

    private void Awake()
    {
        db = new DbRoot();
        LoadGameData();
        InitializeGameData();

        // Inisiasi Hand khusus on progress
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

        // Jika tidak ada is_main pada progress
        if (ProgressLevel.All(p => !p.Get_is_main()))
        {
            Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            Debug.LogWarning("Membuat progress 4 menjadi is_main (default)");
            levelData.SetIsMainLevel(Level);
        }

        // Start Audio sesuai dengan ProgressLevel yang is main
        int index = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (index == 0)
        {
            ManagerAudio.instance.PlayVALevel2(1, 1);
        }
        else if (index == 1)
        {
            ManagerAudio.instance.PlayVALevel2(2, 1);
        }
        else if (index == 2)
        {
            ManagerAudio.instance.PlayVALevel2(3, 1);
        }
        else
        {
            Debug.LogWarning("Index progress dengan is_main = true tidak ditemukan di level " + Level);
        }

    }

    public void OnClickObjek(int nomorGameplay, string namaProgress)
    {
        Debug.Log("Objek diklik: " + gameObject.name);
        Debug.Log("Memulai gameplay nomor: " + nomorGameplay + " dengan progress: " + namaProgress);
        if (nomorGameplay == 4)
        {
            // Gameplay Sikat gigi
            Debug.Log("Memulai gameplay Sikat Gigi (2_1)");

            // Nonaktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(false);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(false);

            // Nonaktifk OnClickErrorObjek
            SetOnClickErrorObjek(false);

            // Putar suara kembali
            ManagerAudio.instance.PlayVALevel2(1, 1);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            /// GAMEPLAY 2_1
            // Ganti background ke opsional 1
            GantiBackgroundOpsional(1);

            // Aktifkan parent gameplay gosok gigi
            if (ParentGameplayGosokGigi != null) ParentGameplayGosokGigi.SetActive(true);

            // Atur sprite karakter sesuai jenis kelamin
            string jenisKelamin = levelData.GetJenisKelamin();

            if (jenisKelamin == "laki-laki")
            {
                if (SpriteKarakterGigi_L != null) SpriteKarakterGigi_L.SetActive(true);
                if (SpriteKarakterGigi_P != null) SpriteKarakterGigi_P.SetActive(false);
            }
            else if (jenisKelamin == "perempuan")
            {
                if (SpriteKarakterGigi_L != null) SpriteKarakterGigi_L.SetActive(false);
                if (SpriteKarakterGigi_P != null) SpriteKarakterGigi_P.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Jenis kelamin tidak dikenali: " + jenisKelamin);
                Debug.LogWarning("Menggunakan sprite karakter laki-laki sebagai default");
                if (SpriteKarakterGigi_L != null) SpriteKarakterGigi_L.SetActive(true);
                if (SpriteKarakterGigi_P != null) SpriteKarakterGigi_P.SetActive(false);
            }

            // Aktifkan HandHelpOdol
            HandHelpOdol.SetActive(true);

            // Mulai animasi tangan untuk ke odol
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpOdol));

            // Aktifkan Sikat gigi
            if (SikatGigi != null) SikatGigi.SetActive(true);

            // Aktifkan Odol
            if (Odol != null) Odol.SetActive(true);

            // Nonaktifkan Cairan Odol, Gigi, Parent Noda gigi
            if (CairanOdol != null) CairanOdol.SetActive(false);
            if (Gigi != null) Gigi.SetActive(false);
            if (ParentListNodaGigi != null) ParentListNodaGigi.SetActive(false);
        }

        else if (nomorGameplay == 5)
        {
            // Gameplay Mandi
            Debug.Log("Memulai gameplay Mandi (2_2)");

            // Nonaktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(false);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(false);

            // Nonaktifk OnClickErrorObjek
            SetOnClickErrorObjek(false);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Putar suara kembali
            ManagerAudio.instance.PlayVALevel2(2, 1);

            /// GAMEPLAY 2_2
            // Ganti background ke opsional 2
            GantiBackgroundOpsional(2);

            // Aktifkan Bak Mandi
            if (BakMandi != null) BakMandi.SetActive(true);

            // Aktifkan parent gameplay mandi
            if (ParentGameplayMandi != null) ParentGameplayMandi.SetActive(true);

            // Aktifkan HandHelpSabun
            HandHelpSabun.SetActive(true);
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpSabun));

            // Aktifkan Shampo juga
            Shampoo.SetActive(true);
            // Aktifkan Gayung juga
            ParentGayung.SetActive(true);

            // Atur sprite karakter sesuai jenis kelamin
            SpriteKarakterMandi("Sabun");

            // AKtifkan Sabun 
            if (Sabun != null) Sabun.SetActive(true);

            // Aktifkan Sabun script
            Sabun.GetComponent<DragObjectSpriteMandi>().enabled = true;
        }

        else if (nomorGameplay == 6)
        {
            // Gameplay Pengering
            Debug.Log("Memulai gameplay Pengering (2_3)");

            // Nonaktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(false);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(false);

            // Nonaktifk OnClickErrorObjek
            SetOnClickErrorObjek(false);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Putar suara kembali
            ManagerAudio.instance.PlayVALevel2(3, 1);

            /// GAMEPLAY 2_3
            /// Ganti background ke opsional 3
            GantiBackgroundOpsional(3);

            // Aktifkan parent gameplay pengering
            if (ParentGameplayPengering != null) ParentGameplayPengering.SetActive(true);
            // Aktifkan Hair Dryer
            if (HairDryer != null) HairDryer.SetActive(true);

            // Aktifkan parent efek air list
            if (ParentEfekAirList != null) ParentEfekAirList.SetActive(true);

            // Aktifkan HandHelpHairDryer
            HandHelpHairDryer.SetActive(true);

            // Stop semua coroutine sebelumnya
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpHairDryer));

            // Atur sprite karakter sesuai jenis kelamin
            if (levelData.GetJenisKelamin() == "laki-laki")
            {
                if (SpriteKarakterPengering_L != null) SpriteKarakterPengering_L.SetActive(true);
                if (SpriteKarakterPengering_P != null) SpriteKarakterPengering_P.SetActive(false);
            }
            else if (levelData.GetJenisKelamin() == "perempuan")
            {
                if (SpriteKarakterPengering_L != null) SpriteKarakterPengering_L.SetActive(false);
                if (SpriteKarakterPengering_P != null) SpriteKarakterPengering_P.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Jenis kelamin tidak dikenali: " + levelData.GetJenisKelamin());
                Debug.LogWarning("Menggunakan sprite karakter laki-laki sebagai default");
                if (SpriteKarakterPengering_L != null) SpriteKarakterPengering_L.SetActive(true);
                if (SpriteKarakterPengering_P != null) SpriteKarakterPengering_P.SetActive(false);
            }
        }
    }

    public void OnProgress(int nomorGameplay, int nomorLevel, string nameDragOpsional)
    {
        Debug.Log("Progress " + nomorGameplay + " diaktifkan");

        if (nomorGameplay == 4 && nomorLevel == 2)
        {

        }
    }

    public void OnSelesaiProgress(int nomorGameplay, int nomorLevel)
    {
        Debug.Log("Progress " + nomorGameplay + " selesai");

        if (nomorGameplay == 41 && nomorLevel == 2)
        {
            // Debug
            Debug.Log("Progress Sikat Gigi (2_1) selesai");

            // Aktifkan Cairan Odol
            if (CairanOdol != null) CairanOdol.SetActive(true);

            // Matikan animasi
            StopAllCoroutines();

            // Nonaktifkan HandHelpOdol
            HandHelpOdol.SetActive(false);

            // Nonaktifkan Odol
            if (Odol != null) Odol.SetActive(false);

            // Aktifkan Gigi dan Parent Noda gigi
            if (Gigi != null) Gigi.SetActive(true);
            if (ParentListNodaGigi != null) ParentListNodaGigi.SetActive(true);

            // Putar Suara
            ManagerAudio.instance.PlayVALevel2(1, 2);

            // Aktifkan Script pada Sikat gigi
            SikatGigi.GetComponent<DragAndCleanGigi>().enabled = true;

            // Aktifkan HandHelpGosokGigi
            HandHelpGosokGigi.SetActive(true);

            // Mulai animasi tangan untuk gosok gigi
            StopAllCoroutines();
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpGosokGigi));
        }
        else if (nomorGameplay == 42 && nomorLevel == 2)
        {
            // Debug
            Debug.Log("Progress Sikat Gigi (2_2) selesai");

            // Panggil OnSelesaiGameplay
            OnSelesaiGameplay(4);
        }
        else if (nomorGameplay == 51 && nomorLevel == 2)
        {
            // Debug
            Debug.Log("Progress Mandi (2_1) selesai");

            // Aktifkan HandHelpShampoo
            HandHelpShampoo.SetActive(true);

            StopAllCoroutines();

            // Nonaktifkan HandHelpSabun
            HandHelpSabun.SetActive(false);

            // Aktifkan Busa Efek pada Sabun
            if (EfekBusaSabun != null) EfekBusaSabun.SetActive(true);

            // Nonaktifkan Gameplay sebelumnya
            Sabun.GetComponent<DragObjectSpriteMandi>().enabled = false;

            // Aktifkan Sprite Karakter Shampoo
            SpriteKarakterMandi("Shampoo");

            // Mulai animasi tangan untuk ke shampoo
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpShampoo));

            // Aktifkan Shampoo script
            Shampoo.GetComponent<DragObjectSpriteMandi>().enabled = true;

            // Putar Suara
            ManagerAudio.instance.PlayVALevel2(2, 2);
        }
        else if (nomorGameplay == 52 && nomorLevel == 2)
        {
            // Debug
            Debug.Log("Progress Mandi (2_2) selesai");

            // Mulai animasi tangan untuk ke gayung
            StopAllCoroutines();

            // Nonaktifkan HandHelpShampoo
            HandHelpShampoo.SetActive(false);

            // Aktifkan Busa Efek pada Shampoo
            if (EfekBusaShampoo != null) EfekBusaShampoo.SetActive(true);

            // Aktifkan Sprite Karakter Gayung
            SpriteKarakterMandi("Gayung");

            // Aktifkan HandHelpGayung
            HandHelpGayung.SetActive(true);
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpGayung));

            // Nonaktifkan Gameplay sebelumnya
            Shampoo.GetComponent<DragObjectSpriteMandi>().enabled = false;

            // Aktifkan Gayung script
            if (ParentGayung != null && ParentGayung.GetComponent<DragMandiAir>() != null)
                ParentGayung.GetComponent<DragMandiAir>().enabled = true;

            // Putar Suara
            ManagerAudio.instance.PlayVALevel2(2, 3);
        }
        else if (nomorGameplay == 53 && nomorLevel == 2)
        {
            // Debug
            Debug.Log("Progress Mandi (2_3) selesai");

            StopAllCoroutines();

            // Nonaktifkan HandHelpGayung
            HandHelpGayung.SetActive(false);

            // Panggil OnSelesaiGameplay
            OnSelesaiGameplay(5);
        }
    }

    public void OnSelesaiGameplay(int nomorGameplay)
    {
        Debug.Log("Gameplay selesai untuk nomor: " + nomorGameplay);

        if (nomorGameplay == 4)
        {
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
        else if (nomorGameplay == 5)
        {
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
        }
        else if (nomorGameplay == 6)
        {
            // Tampilkan window winner
            ShowWindowWinner();

            // Stop semua coroutine sebelumnya
            StopAllCoroutines();

            // Nonaktifkan HandHelpPengering
            HandHelpHairDryer.SetActive(false);

            // Play audio
            ManagerAudio.instance.PlaySFXWinProgress();
            ManagerAudio.instance.PlayVAAfirmasiPositif();

            // Hadiah koin untuk progress ini
            int hadiahKoin = levelData.GetHadiahKoinProgress(ProgressLevel[2].id_progress);
            UpdateKoinPlayer(hadiahKoin);

            int CountMain = MainSession.Count;

            // Update status progress main → selesai
            if (CountMain > 0)
            {
                levelData.UpdateStatusPenyelesaianProgressMain(ProgressLevel[2].id_progress, MainSession[CountMain - 1].id_main, 1);
            }

            // Ubah Koin
            AmountKoinText.GetComponent<TextMeshProUGUI>().text = hadiahKoin.ToString();

            // Update waktu selesai progress
            levelData.UpdateWaktuSelesaiProgressMain(ProgressLevel[2].id_progress);

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

        if (indexAktif == 0 || indexAktif == 1)
        {
            // Mulai Animasi tangan lagi
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(2);

            // Ganti background ke default
            GantiBackgroundOpsional(0);

            // Aktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(true);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(true);

            // Nonaktifkan parent Gameplay sebelumnya (sesuai indexAktif)
            if (indexAktif == 0)
            {
                if (ListGrupObjekGameplay.Count > 0 && ListGrupObjekGameplay[0] != null)
                    ListGrupObjekGameplay[0].SetActive(false);
            }
            else if (indexAktif == 1)
            {
                if (ListGrupObjekGameplay.Count > 1 && ListGrupObjekGameplay[1] != null)
                    ListGrupObjekGameplay[1].SetActive(false);

                // Putar Audio
                ManagerAudio.instance.PlayVALevel2(3, 1);
            }

            // Play Audio (Opsional)
            // ManagerAudio.instance.PlayVALevel2(2, 1);

            // Aktifkan OnClickErrorObjek
            SetOnClickErrorObjek(true);
        }
        else if (indexAktif == 2)
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
    void SpriteKarakterMandi(string namaObjek)
    {
        string jenisKelamin = levelData.GetJenisKelamin();

        if (namaObjek == "Sabun")
        {
            if (jenisKelamin == "laki-laki")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(true);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(false);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(false);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(false);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(false);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(false);
            }
            else if (jenisKelamin == "perempuan")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(false);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(true);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(false);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(false);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(false);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(false);
            }
        }
        else if (namaObjek == "Shampoo")
        {
            if (jenisKelamin == "laki-laki")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(false);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(false);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(true);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(false);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(false);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(false);
            }
            else if (jenisKelamin == "perempuan")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(false);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(false);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(false);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(true);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(false);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(false);
            }
        }
        else if (namaObjek == "Gayung")
        {
            if (jenisKelamin == "laki-laki")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(false);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(false);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(false);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(false);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(true);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(false);
            }
            else if (jenisKelamin == "perempuan")
            {
                if (KarakterMandiSabun_L != null) KarakterMandiSabun_L.SetActive(false);
                if (KarakterMandiSabun_P != null) KarakterMandiSabun_P.SetActive(false);
                if (KarakterMandiShampoo_L != null) KarakterMandiShampoo_L.SetActive(false);
                if (KarakterMandiShampoo_P != null) KarakterMandiShampoo_P.SetActive(false);
                if (KarakterMandiGayung_L != null) KarakterMandiGayung_L.SetActive(false);
                if (KarakterMandiGayung_P != null) KarakterMandiGayung_P.SetActive(true);
            }
        }
        else
        {
            Debug.LogWarning("Nama objek tidak dikenali: " + namaObjek);
        }
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

    private void GantiBackgroundOpsional(int index)
    {
        if (BackgroundCanvas == null || BackgroundOpsional == null || BackgroundOpsional2 == null)
        {
            Debug.LogWarning("Salah satu objek background belum di-assign di inspector.");
            return;
        }

        if (index == 0)
        {
            BackgroundCanvas.SetActive(true);
            BackgroundOpsional.SetActive(false);
            BackgroundOpsional2.SetActive(false);
            BackgroundOpsional3.SetActive(false);
        }
        else if (index == 1)
        {
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional.SetActive(true);
            BackgroundOpsional2.SetActive(false);
            BackgroundOpsional3.SetActive(false);
        }
        else if (index == 2)
        {
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional.SetActive(false);
            BackgroundOpsional2.SetActive(true);
            BackgroundOpsional3.SetActive(false);
        }
        else if (index == 3)
        {
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional.SetActive(false);
            BackgroundOpsional2.SetActive(false);
            BackgroundOpsional3.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Index background opsional tidak dikenali: " + index);
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
