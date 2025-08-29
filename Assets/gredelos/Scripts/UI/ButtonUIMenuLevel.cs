using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode] // supaya script jalan juga di editor
public class ButtonUIMenuLevel : MonoBehaviour
{
    [Header("Scane Pilih Jenis Kelamin")]
    public string NameScenePilihJK = "PilihJenisKelamin";

    [Header("Coin Info")]
    public GameObject CoinAmout;
    
    public static LevelDataController levelData; // akses statis

    private DbRoot levelDbOffline; // akses database offline

    string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    private int jumlah_koin = 0;

    // Jika dalam editor maka ambil dari db yang ada
    private void Update()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying) // hanya jalan di editor mode
        {
            if (File.Exists(FilePath))
            {
                var levelDbOffline = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
                jumlah_koin = levelDbOffline.player[0].jumlah_koin;

                if (CoinAmout != null && CoinAmout.GetComponent<TextMeshProUGUI>() != null)
                {
                    CoinAmout.GetComponent<TextMeshProUGUI>().text = jumlah_koin.ToString();
                }
            }
        }
        #endif  
    }

    public void Awake()
    {
        if (levelData == null)
        {
            levelData = LevelDataController.I;
        }
    }

    public void BackToPilihJK()
    {
        // Load scene pilih jenis kelamin
        UnityEngine.SceneManagement.SceneManager.LoadScene(NameScenePilihJK);
    }

    private void Start()
    {
       if (CoinAmout != null && CoinAmout.GetComponent<TextMeshProUGUI>() != null)
        {
            jumlah_koin = levelData.GetKoinPlayer();
            CoinAmout.GetComponent<TextMeshProUGUI>().text = jumlah_koin.ToString();
        }
    }
}
