using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;

public class ControllerPlayObjekLevel5 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "MenuLevel";

    [Header("Gameplay Manager")]
    public int Level = 5;
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
    public GameObject HandObjekMakan;
    public GameObject HandObjekSabunCuci;

    [Header("Level 5 - Progress 11")]
    public GameObject ParentGameplayMakan;
    public GameObject MejaMakanGameplay;
    public GameObject ParentSendokMakan;
    public GameObject SendokMakan;
    public GameObject MakananSendok;
    public GameObject ColliderMakanan;
    public GameObject ColliderMulut;
    public GameObject SpriteKarakterCowokMakan;
    public GameObject SpriteKarakterCewekMakan;
    public GameObject HandHelpMakan;

    [Header("Level 5 - Progress 12")]
    public GameObject ParentGameplayCuciPiring;
    public GameObject SabunCuci;
    public GameObject SponsDrag;
    public GameObject SponsCuciSprite;
    public GameObject LapDrag;
    public GameObject SpritePiringSabunCuci;
    public GameObject SpriteLapPiring;
    public GameObject AreaLetakPiring;
    public GameObject PiringBersih;

    [Header("HandHelp Progress 12")]
    public GameObject HandHelpSabunCuci;
    public GameObject HandHelpCuciPiring;
    public GameObject HandHelpLapPiring;
    public GameObject HandHelpLetakPiring;

    // Data Progress
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
            Debug.LogWarning("Membuat progress 11 menjadi is_main (default)");
            levelData.SetIsMainLevel(Level);
        }

        // Start Audio sesuai dengan ProgressLevel yang is main
        int index = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (index == 0)
        {
            ManagerAudio.instance.PlayVALevel5(1, 1);
        }
        else if (index == 1)
        {
            ManagerAudio.instance.PlayVALevel5(2, 1);
        }
        
    }

    public void OnClickObjek(int nomorGameplay, string namaProgress)
    {
        if (nomorGameplay == 11)
        {
            Debug.Log("Progress 11 tahap 1 diaktifkan");

            // Ganti Background
            GantiBackgroundOpsional(1);

            // Nonaktifkan OnClickError
            SetOnClickErrorObjek(false);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Nonaktifkan objek parent objek gameplay dan karakter
            ParentObjekGameplay.SetActive(false);
            ParentKarakter.SetActive(false);

            // Tampilkan Gameplay Objek
            ParentGameplayMakan.SetActive(true);
            MejaMakanGameplay.SetActive(true);
            ParentSendokMakan.SetActive(true);
            SendokMakan.SetActive(true);
            MakananSendok.SetActive(false);
            SpriteKarakterCowokMakan.SetActive(false);
            SpriteKarakterCewekMakan.SetActive(false);

            // Suara Instruksi
            ManagerAudio.instance.PlayVALevel5(1, 2);

            // Animasi tangan
            // Stop animasi tangan yang lain
            StopAllCoroutines();
            // Aktifkan
            HandObjekMakan.SetActive(true);
            StartCoroutine(PlayHandAnimation3(3, 2, HandObjekMakan));

            // Jalankan script untuk Progress 11 pertama
            if (SendokMakan != null)
            {
                var klikProgress = ParentSendokMakan.GetComponent<OnClickObjekProgress>();
                if (klikProgress != null)
                {
                    klikProgress.enabled = true;
                }
                else
                {
                    Debug.LogWarning("OnClickObjekProgress tidak ada di SendokMakan!");
                }
            }
            else
            {
                Debug.LogWarning("SendokMakan belum di-assign!");
            }
        }
        else if (nomorGameplay == 12)
        {
            Debug.Log("Progress 12 tahap 1 diaktifkan");

            // Ganti Background
            GantiBackgroundOpsional(2);

            // Nonaktifkan OnClickError
            SetOnClickErrorObjek(false);

            // Start Data Main Session
            levelData.StartLevel_OnClick(Level, namaProgress);

            // Nonaktifkan objek parent objek gameplay dan karakter
            ParentObjekGameplay.SetActive(false);
            ParentKarakter.SetActive(false);

            // Tampilkan Gameplay Objek
            // Aktifkan Parent
            ParentGameplayCuciPiring.SetActive(true);

            // Nonaktifkan SponsDrag,
            SponsDrag.SetActive(false);
            SpriteLapPiring.SetActive(false);
            SpritePiringSabunCuci.SetActive(false);

            // Aktifkan SponsSprite
            SponsCuciSprite.SetActive(true);

            // Aktifkan Script OnProgress pada SponsDrag
            var onProgress = SabunCuci.GetComponent<OnClickObjekProgress>();
            if (onProgress != null)
            {
                onProgress.enabled = true;
            }
            else
            {
                Debug.LogWarning("OnProgress tidak ada di SabunCuci!");
            }

            // Suara Instruksi
            ManagerAudio.instance.PlayVALevel5(2, 2);

            // Animasi tangan
            // Stop animasi tangan yang lain
            StopAllCoroutines();

            // Aktifkan Animasi Hand Piring
            HandObjekSabunCuci.SetActive(true);
            StartCoroutine(PlayHandAnimation3(3, 2, HandObjekSabunCuci));

            

        }
    }

    public void OnProgress(int nomorGameplay, int nomorLevel, string nameDragOpsional)
    {
        Debug.Log("Progress " + nomorGameplay + " diaktifkan");

        if (nomorLevel == 5 && nomorGameplay == 111)
        {
            Debug.Log("Progress 11 tahap 1 diaktifkan");

            // Aktifkan Sprite sesuai dengan jenis kelamin di database, jika tidak ada, defult ke cowok
            if (levelData.GetJenisKelamin() == "laki-laki")
            {
                SpriteKarakterCowokMakan.SetActive(true);
                SpriteKarakterCewekMakan.SetActive(false);
            }
            else if (levelData.GetJenisKelamin() == "perempuan")
            {
                SpriteKarakterCewekMakan.SetActive(true);
                SpriteKarakterCowokMakan.SetActive(false);
            }

            // Nonaktifkan meja
            MejaMakanGameplay.SetActive(false);

            // Putar Instruksi
            ManagerAudio.instance.PlayVALevel5(1, 4);

            // Nonaktifkan HandObjekMakan
            HandObjekMakan.SetActive(false);

            // Nonaktifkan animasi tangan lain
            StopAllCoroutines();

            // Aktifkan Hand Bantuan
            if (HandHelpMakan != null)
            {
                HandHelpMakan.SetActive(true);
                var handAnim = HandHelpMakan.GetComponent<IHandAnim>();
                if (handAnim != null)
                {
                    handAnim.PlayAnimation();
                }
                else
                {
                    Debug.LogWarning("Tidak ada script animasi yang sesuai di HandHelpMakan");
                }
            }
            else
            {
                Debug.LogWarning("HandHelpMakan belum di-assign!");
            }

            // Aktifkan Script DragSendok pada parent Sendok
            var dragSendok = ParentSendokMakan.GetComponent<DragSendok>();
            var OnClickProgress = ParentSendokMakan.GetComponent<OnClickObjekProgress>();
            if (dragSendok != null || OnClickProgress != null)
            {
                dragSendok.enabled = true;
                OnClickProgress.enabled = false;

                if (ColliderMakanan != null && ColliderMulut != null)
                {
                    // Aktifkan collider untuk makanan dan mulut
                    ColliderMakanan.SetActive(true);
                    ColliderMulut.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning("DragSendok atau OnClickProgress tidak ada di ParentSendokMakan!");
            }


        }
        else if (nomorLevel == 5 && nomorGameplay == 121)
        {
            Debug.Log("Progress 12 tahap 1 diaktifkan");

            // Nonaktifkan animasi tangan
            StopAllCoroutines();

            // Nonaktifkan HandObjekSabunCuci
            HandObjekSabunCuci.SetActive(false);

            // Nonaktifkan OnClickProgress
            var onProgress = SabunCuci.GetComponent<OnClickObjekProgress>();
            if (onProgress != null)
            {
                onProgress.enabled = false;
            }
            else
            {
                Debug.LogWarning("OnClickObjekProgress tidak ditemukan di SabunCuci");
            }

            // Aktifkan Script Drag pada SponsSprite
            SabunCuci.GetComponent<DragObjectSprite>().enabled = true;

            // Putar suara
            ManagerAudio.instance.PlayVALevel5(2, 3);

            // Nonaktifkan semua animasi tangan
            StopAllCoroutines();

            // Aktifkan Animasi tangan Help Cuci Piring
            HandHelpSabunCuci.SetActive(true);

            // Putar Animasi
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpSabunCuci));

        }
        else if (nomorLevel == 5 && nomorGameplay == 122)
        {
            Debug.Log("Progress 12 tahap 2 diaktifkan");

            // Nonaktifkan semua animasi tangan
            StopAllCoroutines();

            // Aktifkan HandHelpCuciPiring
            HandHelpCuciPiring.SetActive(true);

            // mainkan Animasi tangan
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpCuciPiring));

            // Nonaktifkan Script gameplay sebelumnya
            // Nonaktifkan Spons Cuci
            SponsCuciSprite.SetActive(false);

            // Nonaktifkan gameplay sabun cuci
            SabunCuci.GetComponent<DragObjectSprite>().enabled = false;
            SabunCuci.GetComponent<OnClickObjekProgress>().enabled = false;

            // Putar suara
            ManagerAudio.instance.PlayVALevel5(2, 4);

            // Aktifkan gameplay saat ini
            SponsDrag.SetActive(true);

            SponsDrag.GetComponent<DragObjectSprite>().enabled = true;

            // Aktifkan SpritePiringSabun
            SpritePiringSabunCuci.SetActive(true);
        }
        else if (nomorLevel == 5 && nomorGameplay == 123)
        {
            Debug.Log("Progress 12 tahap 3 diaktifkan");

            // Nonaktifkan semua animasi Hand
            StopAllCoroutines();

            // Sembunyikan handhelp sebelumnya
            HandHelpCuciPiring.SetActive(false);

            // Sembunyikan Sprite Piring Sabun
            SpritePiringSabunCuci.SetActive(false);

            // Nonaktifkan script gameplay sebelumnya
            SponsDrag.GetComponent<DragObjectSprite>().enabled = false;

            // Aktifkan Gameplay saat ini
            SpriteLapPiring.SetActive(true);

            // Aktifkan Script pada lap
            LapDrag.GetComponent<DragObjectSprite>().enabled = true;

            // Aktifkan HandHelpLapPiring
            HandHelpLapPiring.SetActive(true);

            // Aktifkan animasi
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpLapPiring));

            // Putar suara
            ManagerAudio.instance.PlayVALevel5(2, 5);
        }
        else if (nomorGameplay == 124 && nomorLevel == 5)
        {
            Debug.Log("Progress 12 tahap 4 diaktifkan");

            // Nonaktifkan semua animasi tangan
            StopAllCoroutines();

            // Nonaktifkan HandHelpLapPiring
            HandHelpLapPiring.SetActive(false);

            // Nonaktifkan Script Drag pada Lap
            LapDrag.GetComponent<DragObjectSprite>().enabled = false;

            // Nonaktifkan Sprite Piring Gameplay sebelumnya
            SpriteLapPiring.SetActive(false);

            // Putar suara
            ManagerAudio.instance.PlayVALevel5(2, 6);

            // Aktifkan HandHelpLetakPiring
            HandHelpLetakPiring.SetActive(true);

            // Animasi tangan help
            StartCoroutine(PlayHandAnimationLoop2(4, 2, HandHelpLetakPiring));

            // Aktifkan gameplay
            PiringBersih.SetActive(true);

            // Aktikan Drag Piring
            PiringBersih.SetActive(true);

            // Aktifkan area letak piring
            AreaLetakPiring.SetActive(true);
        }
    }

    public void OnSelesaiProgress(int nomorGameplay, int nomorLevel)
    {
        Debug.Log("Progress " + nomorGameplay + " selesai");

        if (nomorGameplay == 121 && nomorLevel == 5)
        {
            Debug.Log("Progress 12 tahap 1 Selesai");

            // Lanjutkan dengan Progress 12 tahap 2
            OnProgress(122, 5, "OnProgress_5_2");
        }
        else if (nomorGameplay == 122 && nomorLevel == 5)
        {
            Debug.Log("Progress 12 tahap 2 Selesai");

            // Lanjutkan dengan Progress 12 tahap 3
            OnProgress(123, 5, "OnProgress_5_3");
        }
        else if (nomorGameplay == 123 && nomorLevel == 5)
        {
            Debug.Log("Progress 12 tahap 3 Selesai");

            // lanjutkan dengan Progress 12 tahap 4
            OnProgress(124, 5, "OnProgress_5_4");
        }
        else if (nomorGameplay == 124 && nomorLevel == 5)
        {
            Debug.Log("Progress 12 tahap 4 Selesai");

            // Nonaktifkan Parent Gameplay Cuci Piring
            ParentGameplayCuciPiring.SetActive(false);

            // Ganti Background ke default
            GantiBackgroundOpsional(0);

            // Aktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(true);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(true);

            // Stop semua animation tangan
            var handController = FindObjectOfType<LevelHandController>();
            if (handController != null)
                handController.StopAllCoroutines();

            // Panggil OnSelesaiGameplay
            OnSelesaiGameplay(12);
        }
    }

    public void OnSelesaiGameplay(int nomorGameplay)
    {
        Debug.Log("Gameplay selesai untuk nomor: " + nomorGameplay);

        if (nomorGameplay == 11)
        {
            Debug.Log("Progress 11 selesai");

            // Stop All Coroutines
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
        else if (nomorGameplay == 12)
        {
            Debug.Log("Progress 12 Selesai");

            // Stop All Coroutines
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
            // Mulai Animasi tangan lagi
            ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(2);

            // Ganti background ke default
            GantiBackgroundOpsional(0);

            // Aktifkan parent karakter dan parent objek
            if (ParentKarakter != null) ParentKarakter.SetActive(true);
            if (ParentObjekGameplay != null) ParentObjekGameplay.SetActive(true);

            // Nonaktifkan list gameplay 11
            ParentGameplayMakan.SetActive(false);
            MejaMakanGameplay.SetActive(false);
            ParentSendokMakan.SetActive(false);
            SendokMakan.SetActive(false);
            MakananSendok.SetActive(false);
            SpriteKarakterCowokMakan.SetActive(false);
            SpriteKarakterCewekMakan.SetActive(false);

            // Play Audio
            ManagerAudio.instance.PlayVALevel5(2, 1);

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

    private IEnumerator PlayHandAnimation3(int jedaAnimasi, int jedaFirst, GameObject handObject)
    {
        var pointer = handObject.GetComponent<PointerAnimation>();
        if (pointer == null)
        {
            Debug.LogWarning("PointerAnimation tidak ditemukan di " + handObject.name);
            yield break;
        }

        // ambil SpriteRenderer untuk atur transparansi
        var sr = handObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning("SpriteRenderer tidak ditemukan di " + handObject.name);
            yield break;
        }

        // invisible sebelum mulai animasi
        yield return FadeAlpha(sr, 0f, 0f);

        // jeda pertama
        if (jedaFirst > 0)
        {
            yield return new WaitForSeconds(jedaFirst);
        }

        // loop animasi
        while (handObject.activeInHierarchy)
        {
            handObject.SetActive(true);

            // ambil target RectTransform (kalau dipakai di PointerAnimation)
            var rt = handObject.GetComponent<RectTransform>();
            float durasi = 2f; // bisa dijadikan public variable di PointerAnimation

            // Fade in → animasi pointer → Fade out
            yield return FadeAlpha(sr, 0f, 1f, 0.3f); // fade in 0.3 detik
            pointer.PlayAnimation(rt, durasi);
            yield return new WaitForSeconds(durasi);
            yield return FadeAlpha(sr, 1f, 0f, 0.3f); // fade out 0.3 detik

            // jeda antar animasi
            if (jedaAnimasi > 0)
            {
                yield return new WaitForSeconds(jedaAnimasi);
            }
        }
    }

    private IEnumerator FadeAlpha(SpriteRenderer sr, float from, float to, float duration = 0f)
    {
        Color c = sr.color;

        if (duration <= 0f)
        {
            c.a = to;
            sr.color = c;
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, t / duration);
            c.a = alpha;
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
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
        }
        else if (index == 1)
        {
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional.SetActive(true);
            BackgroundOpsional2.SetActive(false);
        }
        else if (index == 2)
        {
            BackgroundCanvas.SetActive(false);
            BackgroundOpsional.SetActive(false);
            BackgroundOpsional2.SetActive(true);
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