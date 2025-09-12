using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class UpToDatabase : MonoBehaviour
{
    public static UpToDatabase instance;
    public float intervalKirimData = 30f;

    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Muat data utama
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                db = JsonUtility.FromJson<DbRoot>(json);
                Debug.Log("Data dimuat dari " + FilePath);
            }
            else
            {
                db = new DbRoot();
                Debug.Log("File data tidak ditemukan, membuat data baru.");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Kirim sekali saat game mulai
        StartCoroutine(UploadDataToServer());

        // Loop kirim tiap interval
        InvokeRepeating(nameof(KirimData), intervalKirimData, intervalKirimData);
    }

    private void KirimData()
    {
        StartCoroutine(UploadDataToServer());
    }

    private IEnumerator UploadDataToServer()
    {
        string jsonData = JsonUtility.ToJson(db, true);
        byte[] postData = Encoding.UTF8.GetBytes(jsonData);

        using (var www = new UnityEngine.Networking.UnityWebRequest("https://api.grodelos.id/api/json/store", "POST"))
        {
            www.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(postData);
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
                Debug.LogError("Error uploading data: " + www.error);
            else
                Debug.Log("Data uploaded successfully: " + www.downloadHandler.text);
        }
    }
}
