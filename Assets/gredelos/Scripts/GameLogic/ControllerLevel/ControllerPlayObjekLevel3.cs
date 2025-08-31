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
    public GameObject KarakterLakiAnimbaju;
    public GameObject KarakterPerempuanAnimbaju;
    public GameObject HandKenakanBajuP;
    public GameObject HandKenakanBajuL;
    public GameObject HandObjekBaju;


    [Header("Level 3 - Progress 8")]
    public GameObject KarakterLakiAnimSisir;
    public GameObject KarakterPerempuanAnimSisir;

    // Data Progress
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    // Data Controller
    public static LevelDataController levelData;

    // List data Progress terkait
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();

    void Awake()
    {
        // Inisialisasi level data
        levelData = LevelDataController.I;
        ProgressLevel = levelData.GetProgressDataByLevel(Level);
    }

    void Start()
    {
        // Mulai animasi tangan pada ManagerGameplay
        ManagerGameplay.GetComponent<LevelHandController2>().StartWithJeda(1); // jeda 5 detik

        // Jika tidak ada is_main pada progress
        if (ProgressLevel.All(p => !p.Get_is_main()))
        {
            Debug.LogWarning("Tidak ada progress dengan is_main = true di level " + Level);
            Debug.LogWarning("Membuat progress 7 menjadi is_main (default)");
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
    }

    public void OnClickObjek(int nomorGameplay, string jenisKelamin)
    {
        if (nomorGameplay == 7)
        {
            // Cek Jenis kelamin, apakah sesuai pada LevelData
            if (levelData.GetJenisKelamin() == jenisKelamin)
            {
                HandleGameplayClick(
                    ListGrupGameplayObject[0], BackgroundOpsional,
                    new List<GameObject> { HandKenakanBajuL, HandKenakanBajuP }, 1, nomorGameplay);
            }
            else
            {
                Debug.LogWarning("Jenis kelamin tidak sesuai.");
            }
        }
        else if (nomorGameplay == 8)
        {
            HandleGameplayClick(ListGrupGameplayObject[1], BackgroundOpsional2, null, 2, nomorGameplay);
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
        }
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
