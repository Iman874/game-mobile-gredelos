using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[ExecuteAlways]
public class UIController : MonoBehaviour
{
    [Header("Database")]
    [Header("Database Info")]
    private DbRoot dbRead;
    private string fileName = "game_data.json";
    string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    // UI controller untuk UI pengaturan dan coin Amount
    // Ambil gameobject
    [Header("UI Elements")]
    public GameObject UISettings;

    public GameObject UICoinAmount;

    public GameObject BackgroundforCoinAmount;

    // fungsi jika Coin Amount lebih dari 10000 maka ubah tambah scale background
    public void UpdateCoinAmountUI(int coinAmount)
    {
        if (coinAmount >= 10000)
        {
            BackgroundforCoinAmount.transform.localScale = new Vector3(1.3f, 1f, 1f);
            BackgroundforCoinAmount.transform.localPosition = new Vector3(-50f, BackgroundforCoinAmount.transform.localPosition.y, BackgroundforCoinAmount.transform.localPosition.z);
        }
        else
        {
            BackgroundforCoinAmount.transform.localScale = new Vector3(1f, 1f, 1f);
            BackgroundforCoinAmount.transform.localPosition = new Vector3(0f, BackgroundforCoinAmount.transform.localPosition.y, BackgroundforCoinAmount.transform.localPosition.z);
        }
    }

    private void OnValidate()
    {
        // Cek gameobject tidak null
        if (UISettings == null || UICoinAmount == null || BackgroundforCoinAmount == null)
        {
            Debug.LogWarning("Salah satu GameObject UI belum diassign di inspector!");
            return;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Cek gameobject tidak null
            if (UISettings == null || UICoinAmount == null || BackgroundforCoinAmount == null)
            {
                Debug.LogWarning("Salah satu GameObject UI belum diassign di inspector!");
                return;
            }

            // set scale background coin amount sesuai coin amount
            // Load database dari file JSON
            dbRead = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
            UpdateCoinAmountUI(dbRead.player[0].jumlah_koin);
        }
#endif
    }
}
