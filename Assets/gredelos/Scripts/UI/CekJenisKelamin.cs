using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class CekJenisKelamin : MonoBehaviour
{
    [Header("Cek Jenis Kelamin")]
    [SerializeField] private GameObject karakterPria;
    [SerializeField] private GameObject karakterWanita;
    public static LevelDataController levelData; // akses statis

    private void Awake()
    {
        if (levelData == null)
        {
            levelData = LevelDataController.I;
        }
    }

    public void CekKarakter()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelDataController tidak ditemukan!");
            return;
        }

        if (levelData.GetJenisKelamin() == "laki-laki")
        {
            Debug.Log("Karakter adalah Pria");
            karakterPria.SetActive(true);
            karakterWanita.SetActive(false);
        }
        else if (levelData.GetJenisKelamin() == "perempuan")
        {
            Debug.Log("Karakter adalah Wanita");
            karakterPria.SetActive(false);
            karakterWanita.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Jenis kelamin tidak dikenali: " + levelData.GetJenisKelamin());
            // Default ke pria
            karakterPria.SetActive(true);
            karakterWanita.SetActive(false);
        }
    }

    private void Start()
    {
        CekKarakter();
    }
}
