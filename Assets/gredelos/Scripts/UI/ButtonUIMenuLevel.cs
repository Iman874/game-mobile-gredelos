using UnityEngine;
using TMPro;

public class ButtonUIMenuLevel : MonoBehaviour
{
    [Header("Scane Pilih Jenis Kelamin")]
    public string NameScenePilihJK = "PilihJenisKelamin";

    [Header("Coin Info")]
    public GameObject CoinAmout;
    
    public static LevelDataController levelData; // akses statis


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
        Update(); // Panggil Update sekali di Start untuk inisialisasi awal
    }

    private void Update()
    {
        // Update jumlah koin di UI
        if (CoinAmout != null && CoinAmout.GetComponent<TextMeshProUGUI>() != null)
        {
            int jumlahKoin = levelData.GetKoinPlayer();
            CoinAmout.GetComponent<TextMeshProUGUI>().text = jumlahKoin.ToString();
        }
    }

}
