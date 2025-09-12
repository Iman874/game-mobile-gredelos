using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[ExecuteAlways]
public class CekJenisKelamin : MonoBehaviour
{
    // Database
    [Header("Database")]
    [Header("Database Info")]
    private DbRoot dbRead;
    private string fileName = "game_data.json";
    string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    [Header("Cek Jenis Kelamin")]
    [SerializeField] private GameObject karakterPria;
    [SerializeField] private GameObject karakterWanita;

    public void CekKarakter()
    {
        // Set karakter sesuai pilihan di database
        if (dbRead.player[0].jenis_kelamin == "laki-laki")
        {
            Debug.Log("Karakter adalah Pria");
            karakterPria.SetActive(true);
            karakterWanita.SetActive(false);
        }
        else if (dbRead.player[0].jenis_kelamin == "perempuan")
        {
            Debug.Log("Karakter adalah Wanita");
            karakterPria.SetActive(false);
            karakterWanita.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Jenis kelamin tidak dikenali: " + dbRead.player[0].jenis_kelamin);
            // Default ke pria
            karakterPria.SetActive(true);
            karakterWanita.SetActive(false);
        }
    }

    private void Start()
    {
        // Load database dari file JSON
        dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));

        CekKarakter();
    }

    private void OnValidate()
    {
        // Cek gameobject tidak null
        if (karakterPria == null || karakterWanita == null)
        {
            Debug.LogWarning("GameObject karakterPria atau karakterWanita belum diassign di inspector!");
            return;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        // Load database dari file JSON
        dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
        
        // hanya di editor, cek perubahan di inspector
        if (!Application.isPlaying)
        {
            CekKarakter();
        }
#endif
    }
}
