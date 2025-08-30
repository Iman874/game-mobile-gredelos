using UnityEngine;
using System.Collections.Generic; // wajib kalau mau pakai List<T>
using TMPro; // wajib kalau mau pakai TextMeshProUGUI

public class OnClickError : MonoBehaviour
{
    // Daftar objek yang dianggap valid (boleh diklik)
    [Header("Whitelist Objects (Valid Click Targets)")]
    public GameObject[] WhiteListObjects;

    [Header("Level Info")]

    // Data level dan progress terkait
    public int nomorLevel = 1;

    // Nomor Fungsi
    public int nomorFungsi = 1;

    // Referensi ke Jumlah Koin di UI
    public GameObject CoinAmountPlayer;

    LevelDataController levelData;
    ManagerAudio managerAudio;

    public List<string> namaProgressList = new List<string>
    {
        "lvl_X_01_nama_progress",
        "lvl_X_02_nama_progress"
    };

    // --- NEW ---
    public float ClickCooldown = 1f; // delay 1 detik
    private float lastClickTime = -999f;

    void Awake()
    {
        // Pastikan LevelDataController sudah ada
        levelData = LevelDataController.I;

        // Pastikan ManagerAudio sudah ada
        managerAudio = ManagerAudio.instance;
    }

    void Update()
    {
        // Cek cooldown
        if (Time.time - lastClickTime < ClickCooldown)
            return;

        // Deteksi klik mouse kiri
        if (UnityEngine.InputSystem.Mouse.current != null &&
            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (nomorFungsi == 1)
            {
                lastClickTime = Time.time; // set cooldown
                CheckClick(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
            }
        }

        // Deteksi tap layar sentuh
        if (UnityEngine.InputSystem.Touchscreen.current != null &&
            UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (nomorFungsi == 1)
            {
                lastClickTime = Time.time; // set cooldown
                CheckClick(UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue());   
            }
        }
    }

    void CheckClick(Vector2 screenPos)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        
        // Ambil id progress dari progress utama (is_main == true)
        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);

        if (hit.collider != null)
        {
            GameObject clickedObj = hit.collider.gameObject;

            bool isValidClick = false;
            bool isAddError = false;

            // Periksa apakah termasuk whitelist
            foreach (var obj in WhiteListObjects)
            {
                if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
                {
                    Debug.Log("Klik valid pada: " + clickedObj.name);
                    isValidClick = true;

                    // Jika yang di klik settings langsung return
                    if (clickedObj.name.ToLower().Contains("settings"))
                    {
                        return;
                    }

                    // Ambil nama objek yang di klik
                    string namaObjek = clickedObj.name;

                    // cek semua progress
                    foreach (string namaProgress in namaProgressList)
                    {
                        bool progress = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);
                        if (progress && namaObjek.ToLower().Contains("objek_01"))
                        {
                            Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                            return; // valid, langsung return
                        }
                        else
                        {
                            Debug.LogWarning($"[OnClickError] Progress '{namaProgress}' pada level {nomorLevel} tidak valid (is_main == false).");
    
                            if (isAddError == false)
                            {
                                // Catat kesalahan klik
                                if (namaObjek.ToLower().Contains("objek_02"))
                                {
                                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_" + namaObjek, 1);
                                    isAddError = true; // pastikan hanya sekali saja
                                    Debug.LogWarning("Salah Urutan, kurangi koin " + nomorLevel);
                                    if (managerAudio != null)
                                        managerAudio.PlayVAError();
                                    KurangiKoin(2); // Pengurangan koin 1 x per progress tidak valid
                                }

                                if (namaObjek.ToLower().Contains("objek_03"))
                                {
                                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_" + namaObjek, 1);
                                    isAddError = true; // pastikan hanya sekali saja
                                    Debug.LogWarning("Salah Urutan, kurangi koin " + nomorLevel);
                                    if (managerAudio != null)
                                        managerAudio.PlayVAError();
                                    KurangiKoin(1); // Pengurangan koin 1 x per progress tidak valid
                                }
                            }
                        }
                    }
                    return; // sudah validasi, keluar dari function
                }
            }

            if (isValidClick) return;
            else
            {
                Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);
                if (managerAudio != null)
                    managerAudio.PlayVAError2();
                KurangiKoin(1);

                // Catat kesalahan klik
                levelData.AddErrorToDatabase(id_progress, "klik_salah_level_1", 1);
            }
        }
    }

    // Kurangi koin player
    void KurangiKoin(int jumlah)
    {
        if (levelData != null)
        {
            levelData.KurangiKoin(jumlah);
            if (CoinAmountPlayer != null)
                CoinAmountPlayer.GetComponent<TextMeshProUGUI>().text = levelData.GetKoinPlayer().ToString();
        }
    }
}
