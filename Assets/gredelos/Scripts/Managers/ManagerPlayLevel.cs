using Unity;
using UnityEngine;
using TMPro;
using System.IO;

[ExecuteAlways]
public class ManagerPlayLevel : MonoBehaviour
{
    // Database
    [Header("Database")]
    [Header("Database Info")]
    private DbRoot dbRead;

    public string fileName = "game_data.json";

    string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    [Header("Level Info")]
    public int LevelIndex; // Index level ini, mulai dari 1

    [Header("Coin Info")]
    public GameObject CoinAmount; // Coin yang didapat saat level ini selesai

    [Header("Animasi Karakter")]

    public GameObject ParentKarakter; // Animasi karakter diam

    public GameObject KarakterLakiLaki; // Animasi karakter laki-laki

    public GameObject KarakterPerempuan; // Animasi karakter perempuan

    void Start()
    {
        // Load database dari file JSON
        dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));

        // Set karakter sesuai pilihan di database
        SetKarakter(dbRead);
        // Set UI Coin
        SetCoinUI(dbRead);
    }

    // Tetap perbarui UI dan animasi karakter walau di editor
    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Pastikan database ter-load
            if (File.Exists(FilePath))
            {
                dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
                SetKarakter(dbRead);
                SetCoinUI(dbRead);
            }
            else
            {
                Debug.LogWarning("File database tidak ditemukan di: " + FilePath);
                return;
            }
        }
#endif
    }

    void OnValidate()
    {
        // set karakter dan coin amount sesuai pilihan di database
        if (File.Exists(FilePath))
        {
            dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
            SetKarakter(dbRead);
            SetCoinUI(dbRead);
        }
        else
        {
            Debug.LogWarning("File database tidak ditemukan di: " + FilePath);
        }
    }

    void SetKarakter(DbRoot dbRead)
    {
        // set karakter sesuai pilihan di database
        if (dbRead != null)
        {
            if (dbRead.player[0].jenis_kelamin == "laki-laki")
            {
                // Karakter laki-laki
                KarakterLakiLaki.SetActive(true);
                KarakterPerempuan.SetActive(false);
                Debug.Log("Karakter laki-laki dipilih.");
            }
            else if (dbRead.player[0].jenis_kelamin == "perempuan")
            {
                // Karakter perempuan
                KarakterLakiLaki.SetActive(false);
                KarakterPerempuan.SetActive(true);
                Debug.Log("Karakter perempuan dipilih.");
            }
            else
            {
                // Default, karakter laki-laki
                KarakterLakiLaki.SetActive(true);
                KarakterPerempuan.SetActive(false);
                Debug.LogWarning("Pilihan jenis kelamin tidak valid. Menggunakan karakter laki-laki sebagai default.");
            }
        }
    }

    void SetCoinUI(DbRoot dbRead)
    {
        // Perbarui UI Coin 
        // Ambil data coin dari database
        if (dbRead != null && CoinAmount != null)
        {
            int currentCoins = dbRead.player[0].jumlah_koin;
            CoinAmount.GetComponent<TextMeshProUGUI>().text = currentCoins.ToString();
            Debug.Log("Jumlah koin saat ini: " + currentCoins);
        }
    }
}
