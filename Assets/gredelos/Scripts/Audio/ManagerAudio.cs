using UnityEngine;

public class ManagerAudio : MonoBehaviour
{
    public static ManagerAudio instance;

    [Header("Audio Objects (BGM)")]
    public GameObject BGMOpening; // Masukkan GameObject yang punya AudioSource
    public GameObject BGMLevel1;
    public GameObject BGMLevel2;
    public GameObject BGMLevel3;
    public GameObject BGMLevel4;
    public GameObject BGMLevel5;

    [Header("Audio Objects (SFX)")]
    public GameObject SFXClick;
    public GameObject SFXButtonClick;
    public GameObject SFXSwipe;
    public GameObject SFXWinProgress;
    public GameObject SFXWinLevel;

    [Header("Audio Objects (Voice Actor)")]
    public GameObject VAOpening;
    public GameObject VAMenuLevel;
    public GameObject VAEnding;

    [Header("Audio Objects (Voice Actor Afirmasi Positif)")]
    public GameObject VAAfirmasiPositif1;
    public GameObject VAAfirmasiPositif2;
    public GameObject VAAfirmasiPositif3;
    public GameObject VAAfirmasiPositif4;
    public GameObject VAAfirmasiPositif5;

    [Header("Audio Objects (Voice Actor Afirmasi Negatif)")]
    public GameObject VAAfirmasiNegatif1;
    public GameObject VAAfirmasiNegatif2;
    public GameObject VAAfirmasiNegatif3;
    public GameObject VAAfirmasiNegatif4;

    [Header("Audio Objects (Voice Actor Level 1 Progress)")]
    public GameObject VALevel1Progress1;
    public GameObject VALevel1Progress2;
    public GameObject VALevel1Progress3;

    [Header("Audio Objects (Voice Actor Level 2 Progress)")]
    public GameObject VALevel2Progress1;
    public GameObject VALevel2Progress2;
    public GameObject VALevel2Progress3;

    [Header("Audio Objects (Voice Actor Level 3 Progress)")]
    public GameObject VALevel3Progress1;
    public GameObject VALevel3Progress2;

    [Header("Audio Objects (Voice Actor Level 4 Progress)")]
    public GameObject VALevel4Progress1;
    public GameObject VALevel4Progress2;

    [Header("Audio Objects (Voice Actor Level 5 Progress)")]
    public GameObject VALevel5Progress1;
    public GameObject VALevel5Progress2;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static ManagerAudio GetInstance()
    {
        if (instance == null)
        {
            GameObject prefab = Resources.Load<GameObject>("ManagerAudio"); // nama prefab
            instance = Instantiate(prefab).GetComponent<ManagerAudio>();
        }
        return instance;
    }

    void Start()
    {
        // Mainkan BGM sesuai dengan nama scane
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "MainMenu":
                StopAllBGM();
                PlayAudio(BGMOpening, true);
                PlayAudio(VAOpening, false);
                break;
            case "Level 1 - Kamar Tidur":
                StopAllBGM();
                PlayAudio(BGMLevel1, true);
                break;
            case "Level 2 - Kamar Mandi":
                StopAllBGM();
                PlayAudio(BGMLevel2, true);
                break;
            case "Level 3 - Ruang Ganti":
                StopAllBGM();
                PlayAudio(BGMLevel3, true);
                break;
            case "Level 4 - Ruang Bermain":
                StopAllBGM();
                PlayAudio(BGMLevel4, true);
                break;
            case "Level 5 - Ruang Makan":
                StopAllBGM();
                PlayAudio(BGMLevel5, true);
                break;
            default:
                Debug.LogWarning("Scene tidak dikenali: " + sceneName);
                break;
        }

        // Debug
        Debug.Log("ManagerAudio siap! Instance: " + instance);
    }

    private void StopAllBGM()
    {
        StopAudio(BGMOpening);
        StopAudio(BGMLevel1);
        StopAudio(BGMLevel2);
        StopAudio(BGMLevel3);
        StopAudio(BGMLevel4);
        StopAudio(BGMLevel5);
    }

    public void CekScaneAktif(int nomorLevel)
    {
        if (nomorLevel == 0)
        {
            // Nonaktifkan semua audio BGM level
            StopAllBGM();

            // Mainkan BGMOpening
            PlayAudio(BGMOpening, true);
            Debug.Log("Scane aktif saat ini adalah: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        if (nomorLevel == 1)
        {
            // Nonaktifkan semua audio BGM level
            StopAllBGM();

            // Mainkan BGMLevel1
            PlayAudio(BGMLevel1, true);
            Debug.Log("Scane aktif saat ini adalah: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    /// Play audio dari GameObject
    public void PlayAudio(GameObject audioObj, bool loop = true)
    {
        if (audioObj == null) return;

        AudioSource source = audioObj.GetComponent<AudioSource>();
        if (source == null) return;

        source.loop = loop;
        if (!source.isPlaying) source.Play();
    }

    /// Stop audio dari GameObject
    public void StopAudio(GameObject audioObj)
    {
        if (audioObj == null) return;

        AudioSource source = audioObj.GetComponent<AudioSource>();
        if (source == null) return;

        if (source.isPlaying) source.Stop();
    }

    // Fungsi stop semua audio kecuali SFX
    public void StopAllVA()
    {
        StopAudio(VAOpening);
        StopAudio(VAMenuLevel);
        StopAudio(VAEnding);

        StopAudio(VAAfirmasiPositif1);
        StopAudio(VAAfirmasiPositif2);
        StopAudio(VAAfirmasiPositif3);
        StopAudio(VAAfirmasiPositif4);
        StopAudio(VAAfirmasiPositif5);

        StopAudio(VAAfirmasiNegatif1);
        StopAudio(VAAfirmasiNegatif2);
        StopAudio(VAAfirmasiNegatif3);
        StopAudio(VAAfirmasiNegatif4);

        StopAudio(VALevel1Progress1);
        StopAudio(VALevel1Progress2);
        StopAudio(VALevel1Progress3);

        StopAudio(VALevel2Progress1);
        StopAudio(VALevel2Progress2);
        StopAudio(VALevel2Progress3);

        StopAudio(VALevel3Progress1);
        StopAudio(VALevel3Progress2);

        StopAudio(VALevel4Progress1);
        StopAudio(VALevel4Progress2);

        StopAudio(VALevel5Progress1);
        StopAudio(VALevel5Progress2);
    }

    // Fungsi Button Click SFX
    public void PlaySFXButtonClick()
    {
        PlayAudio(SFXButtonClick, false);
    }

    // Fungsi Click Objek SFX
    public void PlaySFXClick()
    {
        PlayAudio(SFXClick, false);
    }

    // Fungsi VA Opening
    public void PlayVAOpening()
    {
        // pastikan hanya satu VA yang sedang diputar
        StopAllVA();
        PlayAudio(VAOpening, false);
    }

    // Fungsi VA Menu Level
    public void PlayVAMenuLevel()
    {
        // pastikan hanya satu VA yang sedang diputar
        StopAllVA();

        // Mainkan VA Menu Level
        PlayAudio(VAMenuLevel, false);
    }

    // Fungsi VA Intruksi level 1 Progress 1
    public void PlayVALevel1Progress1()
    {
        // Pastikan hanya satu VA yang sedang diputar
        StopAllVA();

        PlayAudio(VALevel1Progress1, false);
    }

    // Fungsi VA Intruksi level 1 Progress 2
    public void PlayVALevel1Progress2()
    {
        // Stop VA Level 1 Progress 1 jika masih diputar
        StopAllVA();
        PlayAudio(VALevel1Progress2, false);
    }

    // Fungsi VA Intruksi level 1 Progress 3
    public void PlayVALevel1Progress3()
    {
        // Stop VA Level 1 Progress 2 jika masih diputar
        StopAllVA();
        PlayAudio(VALevel1Progress3, false);
    }

    // Fungsi SFX Win Progress
    public void PlaySFXWinProgress()
    {
        PlayAudio(SFXWinProgress, false);
    }

    // Fungsi SFX Win Level
    public void PlaySFXWinLevel()
    {
        PlayAudio(SFXWinLevel, false);
    }

    // Fungsi VA Afirmasi Positif (acak 1 dari 5 kecuali 3)
    public void PlayVAAfirmasiPositif()
    {
        // Pastikan hanya satu VA yang sedang diputar
        StopAllVA();

        int rand = Random.Range(1, 6); // 1 sampai 5
        switch (rand)
        {
            case 1:
                PlayAudio(VAAfirmasiPositif1, false);
                break;
            case 2:
                PlayAudio(VAAfirmasiPositif2, false);
                break;
            case 3:
                PlayAudio(VAAfirmasiPositif2, false);
                break;
            case 4:
                PlayAudio(VAAfirmasiPositif4, false);
                break;
            case 5:
                PlayAudio(VAAfirmasiPositif5, false);
                break;
            default:
                break;
        }
    }

    // Fungsi VA Afirmasi Positif (Menyelesaikan Level)
    public void PlayVAAfirmasiPositif_LevelComplete()
    {
        // Pastikan hanya satu VA yang sedang diputar
        StopAllVA();

        // Mainkan VAAfirmasiPositif3
        PlayAudio(VAAfirmasiPositif3, false);
    }

    // Fungsi VA Afirmasi Negatif ()
     // Putar VA Error
    public void PlayVAError()
    {
        // Pastikan hanya satu VA yang sedang diputar
        // Stop semua VA lainnya
        StopAllVA();
        PlayAudio(VAAfirmasiNegatif1, false);
    }

    // Putar VA Error 2
    public void PlayVAError2()
    {
        // Pastikan hanya satu VA yang sedang diputar
        // Stop semua VA lainnya
        StopAllVA();
        PlayAudio(VAAfirmasiNegatif2, false);
    }
}
