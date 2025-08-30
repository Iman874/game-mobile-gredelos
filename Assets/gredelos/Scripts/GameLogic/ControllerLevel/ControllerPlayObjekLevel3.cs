using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;


public class ControllerPlayObjekLevel3 : MonoBehaviour
{
    [Header("Scane Menu Pilihan Level")]
    public string ScanePilihanLevel = "PilihanLevel";

    [Header("Gameplay Manager")]
    public int Level = 1;
    public GameObject ParentObjekGameplay;
    public GameObject ManagerGameplay; // manager gameplay

    [Header("List Objek Gameplay")]
    public List<GameObject> ListGrupObjekGameplay; // list objek gameplay

    [Header("List Objek After Gameplay")]
    public List<GameObject> ListObjekAfterGameplay; // list objek after gameplay

    [Header("Window")]
    public GameObject WindowObjekParent; // objek window
    public GameObject ShadowBackground; // objek shadow background
    public GameObject WindowWinner; // objek window menang

    public GameObject WindowComplateLevel; // objek window complete level

    public GameObject AmountKoinText; // objek text jumlah koin

    [Header("UI")]
    public GameObject AmountKoinPlayer; // objek text jumlah koin player

    [Header("Objek Gameplay")]
    public GameObject BackgroundCanvas; // Background canvas
    public GameObject BackgroundOpsional; // Background gameplay
    public GameObject BackgroundOpsional2; // Background gameplay 2
    public GameObject ParentKarakter; // Background default
    public bool ChangeBackground;
    public GameObject GameplayObjek1;
    // Hand Help
    public GameObject hand1;
    public GameObject GameplayObjek2;
    // Hand Help
    public GameObject hand2;
    public GameObject GameplayObjek3;
    // Hand Help
    public GameObject hand3;
    public static LevelDataController levelData;
    public List<Progress> ProgressLevel = new();
    public List<ProgressMain> ProgressMain = new();
    public List<MainSession> MainSession = new();
    private List<Coroutine> runningCoroutines = new();

    // Data Progress
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    void Start()
    {
        // Mulai animasi tangan pada ManagerGameplay
        ManagerGameplay.GetComponent<LevelHandController>().StartWithJeda(5); // jeda 5 detik

        // Update koin player di UI
        if (AmountKoinPlayer != null)
        {
            int currentKoin = levelData.GetKoinPlayer();
            AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text = currentKoin.ToString();
        }
        else
        {
            Debug.LogWarning("AmountKoinPlayer belum di-assign di inspector.");
        }
    }

    // Fungsi yang dipanggil saat objek di klik
    public void OnClickObjek(int nomorGameplay)
    {
        Debug.Log("Memulai gameplay nomor: " + nomorGameplay);

        if (nomorGameplay == 7)
        {
            // Nonaktifkan parent objek gameplay
            ParentObjekGameplay.SetActive(false);

        }
    }

    // Fungsi yang dipanggil saat gameplay selesai
    public void OnSelesaiGameplay(int nomorGameplay)
    {
        Debug.Log("Selesai gameplay nomor: " + nomorGameplay);
    }

    // Fungsi untuk tombol lanjut di window
    public void NextProgress()
    {
        // Cek progress level mana yang aktif (is_main == true)
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
        if (indexAktif == -1)
        {
            Debug.LogWarning("Tidak ada progress dengan is_main == true ditemukan.");
            return;
        }
        Debug.Log("Progress aktif ditemukan di index: " + indexAktif);

    }

    public void UlangiLevel()
    {

    }

    public void NextLevel()
    {

    }

    private IEnumerator PlayHandAnimationLoop(int jedaAnimasi, int jedaFirst, int nomorHand)
    {
        if (nomorHand == 1)
        {
            // pastikan hand1 tidak terlihat sebelum animasi dimulai
            var sr = hand1.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // invisible dulu
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
            }
            // jeda pertama kali (sekali aja)
            if (jedaFirst > 0)
            {
                yield return new WaitForSeconds(jedaFirst);
            }

            // loop animasi terus
            while (true)
            {
                if (hand1 != null)
                {
                    HandHelp anim = hand1.GetComponent<HandHelp>();
                    if (anim != null)
                    {
                        // Aktifkan hand1
                        anim.PlayAnimation(); // panggil method di script HandHelp
                        Debug.Log("Animasi tangan diputar");
                    }
                }

                yield return new WaitForSeconds(jedaAnimasi);
            }
        }
    }

    public void UpdateKoinPlayer(int jumlahKoin)
    {
        if (AmountKoinPlayer != null)
        {
            int currentKoin = int.Parse(AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text);
            int newKoin = currentKoin + jumlahKoin;
            AmountKoinPlayer.GetComponent<TextMeshProUGUI>().text = newKoin.ToString();

            // Update ke LevelDataController
            levelData.UpdateKoinPlayer(newKoin);
        }
        else
        {
            Debug.LogWarning("AmountKoinPlayer belum di-assign di inspector.");
        }
    }


}
