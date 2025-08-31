using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ControllerPlayObjekLevel2 : MonoBehaviour
{
    [Header("Gameplay Manager")]
    public int Level = 2;
    public GameObject ManagerGameplay; // manager gameplay

    [Header("List Objek Gameplay")]
    public List<GameObject> ListGrupObjekGameplay; // list objek gameplay

    [Header("List Objek After Gameplay")]
    public List<GameObject> ListObjekAfterGameplay; // list objek after gameplay

    [Header("UI")]
    public GameObject AmountKoinPlayer; // objek text jumlah koin player

    void Awake()
    {
        // Inisialisasi level data
    }

    void Start()
    {
        // Update koin player di UI
        if (AmountKoinPlayer != null)
        {
            int currentKoin = 0; // Ambil jumlah koin saat ini
            AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text = currentKoin.ToString();
        }
        else
        {
            Debug.LogWarning("AmountKoinPlayer belum di-assign di inspector.");
        }
    }

    public void OnClickObjek(int nomorGameplay)
    {
        if (nomorGameplay == 1)
        {
            HandleGameplayClick(ListGrupObjekGameplay[0], null, null, 1, nomorGameplay);
        }
        else if (nomorGameplay == 2)
        {
            HandleGameplayClick(ListGrupObjekGameplay[1], null, null, 2, nomorGameplay);
        }
    }

    private void HandleGameplayClick(GameObject obj, GameObject background, List<GameObject> afterGameplay, int progressIndex, int nomorGameplay)
    {
        if (obj != null)
        {
            obj.SetActive(true);
        }
    }
}
