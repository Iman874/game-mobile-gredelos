using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CekJenisKelaminObject : MonoBehaviour
{
    [Header("List Object Laki-laki")]
    public GameObject[] ListObjectLakiLaki;

    [Header("List Object Perempuan")]
    public GameObject[] ListObjectPerempuan;

    // Database json
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    private void LoadDatabase()
    {
        if (File.Exists(FilePath))
        {
            db = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
        }
        else
        {
            Debug.LogError("File not found: " + FilePath);
        }
    }

    public void CheckGender()
    {
        LoadDatabase();

        if (db != null && db.player.Count > 0)
        {
            if (db.player[0].jenis_kelamin == "laki-laki")
            {
                foreach (var obj in ListObjectLakiLaki)
                    if (obj != null) obj.SetActive(true);

                foreach (var obj in ListObjectPerempuan)
                    if (obj != null) obj.SetActive(false);
            }
            else if (db.player[0].jenis_kelamin == "perempuan")
            {
                foreach (var obj in ListObjectLakiLaki)
                    if (obj != null) obj.SetActive(false);

                foreach (var obj in ListObjectPerempuan)
                    if (obj != null) obj.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Jenis kelamin tidak dikenali: " + db.player[0].jenis_kelamin);
            }
        }
    }
}
