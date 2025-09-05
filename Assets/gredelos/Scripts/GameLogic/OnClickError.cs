using UnityEngine;
using System.Collections.Generic; // wajib kalau mau pakai List<T>
using TMPro; // wajib kalau mau pakai TextMeshProUGUI
using UnityEngine.EventSystems; // wajib kalau mau deteksi UI
using System.Linq;

public class OnClickError : MonoBehaviour
{
    public LayerMask gameplayLayer; // assign di Inspector

    // Daftar objek yang dianggap valid (boleh diklik)
    [Header("Whitelist Objects (Valid Click Targets)")]
    public GameObject[] WhiteListObjects;

    public GameObject[] WhiteListUI;

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

    [Header("Modal / Windows")]
    public List<GameObject> ModalRoots = new List<GameObject>(); // drag root Settings, Pause, dsb. ke sini

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

    void Start()
    {
        AddUIWhitelist();
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
            HandleClick(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        }

        // Deteksi tap layar sentuh
        if (UnityEngine.InputSystem.Touchscreen.current != null &&
            UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            HandleClick(UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    private void HandleClick(Vector2 pos)
    {
        // --- 1. Raycast ke Gameplay dulu ---
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(pos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        // (termasuk kalau panel blockernya lupa di-whitelist / tidak raycastable)
        if (IsAnyModalOpen())
        {
            Debug.Log("Modal terbuka → abaikan klik di belakangnya");
            return;
        }

        if (hit.collider != null)
        {
            Debug.Log("Klik Gameplay → lanjutkan cek progress");

            lastClickTime = Time.time;

            if (nomorFungsi == 1)
                CheckClick(pos);
            else if (nomorFungsi == 2)
                CheckClick2(pos);
            else if (nomorFungsi == 3)
                CheckClick3(pos);
            else if (nomorFungsi == 4)
                CheckClick4(pos);
            else if (nomorFungsi == 5)
                CheckClick5(pos);

            return; // <<< jangan cek UI lagi kalau kena Gameplay
        }

        // --- 2. Kalau tidak kena Gameplay, baru cek UI ---
        var uiCheck = CheckUI(pos);

        if (uiCheck == UICheckResult.Whitelist)
        {
            Debug.Log("Klik UI whitelist → abaikan");
            return; // Tidak error, tidak proses
        }
        else if (uiCheck == UICheckResult.NonWhitelist)
        {
            Debug.Log("Klik UI non-whitelist → SALAH!");
            CatatKesalahanUI(nomorLevel);
            return; // Error
        }
    }


    void CatatKesalahanUI(int nomorLevel)
    {
        // Catat kesalahan klik
        managerAudio?.PlayVAError2();
        KurangiKoin(1);
        levelData.AddErrorToDatabaseLevel(nomorLevel, "salah_klik_UI", 1);
    }

    void CheckClick(Vector2 screenPos)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        // Ambil id progress dari progress utama (is_main == true)
        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);

        if (hit.collider == null)
        {
            Debug.Log("Tidak ada objek yang di klik.");
            return;
        }

        GameObject clickedObj = hit.collider.gameObject;
        string namaObjek = clickedObj.name.ToLower();

        bool isValidClick = false;
        bool isAddError = false;

        // Periksa apakah termasuk whitelist
        foreach (var obj in WhiteListObjects)
        {
            if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
            {
                Debug.Log("Klik valid pada: " + clickedObj.name);
                isValidClick = true;

                // Jika klik pada Settings, langsung keluar
                if (namaObjek.Contains("settings"))
                {
                    return;
                }

                // Cek progress
                foreach (string namaProgress in namaProgressList)
                {
                    bool progress = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);

                    if (progress && namaObjek.Contains("objek_01"))
                    {
                        Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                        return; // valid → langsung keluar
                    }
                    else
                    {
                        Debug.LogWarning($"[OnClickError] Progress '{namaProgress}' pada level {nomorLevel} tidak valid (is_main == false).");

                        if (!isAddError)
                        {
                            if (namaObjek.Contains("objek_02") || namaObjek.Contains("objek_03"))
                            {
                                levelData.AddErrorToDatabase(id_progress, "salah_urutan_" + clickedObj.name, 1);
                                isAddError = true; // hanya sekali dicatat

                                Debug.LogWarning("Salah Urutan, kurangi koin " + nomorLevel);
                                if (managerAudio != null)
                                    managerAudio.PlayVAError();

                                KurangiKoin(2); // penalti salah urutan
                            }
                        }
                    }
                }
                return; // sudah validasi semua progress
            }
        }

        // Kalau bukan whitelist
        if (!isValidClick)
        {
            Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);

            if (managerAudio != null)
                managerAudio.PlayVAError2();

            KurangiKoin(1); // penalti klik salah

            // Catat kesalahan klik
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_" + nomorLevel, 1);
        }
    }

    void CheckClick2(Vector2 screenPos)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);
        if (hit.collider == null) return;

        GameObject clickedObj = hit.collider.gameObject;
        string namaObjek = clickedObj.name.ToLower();
        bool isAddError = false;

        // ================= 1. Cek Whitelist UI =================
        if (namaObjek.Contains("settings") || namaObjek.Contains("ui"))
        {
            Debug.Log("Klik UI whitelist → abaikan");
            return;
        }

        // ================= 2. Cek Whitelist Dunia =================
        bool isWhitelistWorld = false;
        foreach (var obj in WhiteListObjects)
        {
            if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
            {
                isWhitelistWorld = true;
                break;
            }
        }

        if (!isWhitelistWorld)
        {
            Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_2", 1);
            return;
        }

        // ================= 3. Cek progress aktif untuk objek =================
        foreach (string namaProgress in namaProgressList)
        {
            bool progressAktif = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);

            if (namaObjek.Contains("objek_01"))
            {
                if (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_2_01_sikat_gigi"))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_01", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_01, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }

            if (namaObjek.Contains("objek_02") || namaObjek.Contains("objek_03"))
            {
                if ((progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_2_02_mandi"))
                    || (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_2_03_mengeringkan_rambut")))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_" + clickedObj.name, 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan " + clickedObj.name + ", kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }
        }

        // ================= 4. Kalau whitelist tapi tidak ada progress aktif =================
        if (!isAddError)
        {
            Debug.LogWarning($"[OnClickError] Tidak ada progress cocok di level {nomorLevel} untuk objek {namaObjek}");
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_2", 1);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
        }
    }

    void CheckClick3(Vector2 clickPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);

        if (hit.collider == null) return;

        GameObject clickedObj = hit.collider.gameObject;
        string namaObjek = clickedObj.name.ToLower();
        bool isAddError = false;

        // ================= 1. Cek Whitelist UI =================
        if (namaObjek.Contains("settings") || namaObjek.Contains("ui"))
        {
            Debug.Log("Klik UI whitelist → abaikan");
            return;
        }

        // ================= 2. Cek Whitelist Dunia =================
        bool isWhitelistWorld = false;
        foreach (var obj in WhiteListObjects)
        {
            if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
            {
                isWhitelistWorld = true;
                break;
            }
        }

        if (!isWhitelistWorld)
        {
            Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_3", 1);
            return;
        }

        // ================= 3. Cek progress dengan kondisi jenis kelamin =================
        string jenisKelamin = levelData.GetJenisKelamin();
        string jk = (jenisKelamin == "perempuan") ? "perempuan" : "laki"; // default laki

        foreach (string namaProgress in namaProgressList)
        {
            bool progressAktif = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);

            if (namaObjek.Contains("objek_01"))
            {
                if (progressAktif && namaObjek.Contains(jk))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_01", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_01, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }

            if (namaObjek.Contains("objek_02"))
            {
                if (progressAktif)
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_02", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_02, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }
        }

        // ================= 4. Kalau whitelist tapi tidak ada progress aktif =================
        if (!isAddError)
        {
            Debug.LogWarning($"[OnClickError] Tidak ada progress cocok di level {nomorLevel} untuk objek {namaObjek}");
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_3", 1);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
        }
    }

    void CheckClick4(Vector3 clickPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(clickPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);

        if (hit.collider == null) return;

        GameObject clickedObj = hit.collider.gameObject;
        string namaObjek = clickedObj.name.ToLower();
        bool isAddError = false;

        // ================= 1. Cek Whitelist UI =================
        if (namaObjek.Contains("settings") || namaObjek.Contains("ui"))
        {
            Debug.Log("Klik UI whitelist → abaikan");
            return;
        }

        // ================= 2. Cek Whitelist Dunia =================
        bool isWhitelistWorld = false;
        foreach (var obj in WhiteListObjects)
        {
            if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
            {
                isWhitelistWorld = true;
                break;
            }
        }

        if (!isWhitelistWorld)
        {
            // Objek bukan whitelist dunia → error umum
            Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_4", 1);
            return;
        }

        // ================= 3. Cek apakah objek whitelist ada progress is_main =================
        foreach (string namaProgress in namaProgressList)
        {
            bool progressAktif = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);

            if (namaObjek.Contains("objek_01"))
            {
                if (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_4_01_merapikan_mainan"))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_01", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_01, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }

            if (namaObjek.Contains("objek_02"))
            {
                if (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_4_02_membersihkan_ruangan"))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_02", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_02, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }
        }

        // ================= 4. Kalau whitelist tapi tidak ada progress aktif =================
        if (!isAddError)
        {
            Debug.LogWarning($"[OnClickError] Tidak ada progress cocok di level {nomorLevel} untuk objek {namaObjek}");
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_4", 1);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
        }
    }

    void CheckClick5(Vector2 screenPos)
    {
        // Implementasi untuk cek klik di fungsi 5
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, gameplayLayer);

        string id_progress = levelData.GetIdProgressIsMainByLevel(nomorLevel);

        if (hit.collider == null) return;

        GameObject clickedObj = hit.collider.gameObject;
        string namaObjek = clickedObj.name.ToLower();
        bool isAddError = false;

        // ================= 1. Cek Whitelist UI =================
        if (namaObjek.Contains("settings") || namaObjek.Contains("ui"))
        {
            Debug.Log("Klik UI whitelist → abaikan");
            return;
        }

        // ================= 2. Cek Whitelist Dunia =================
        bool isWhitelistWorld = false;
        foreach (var obj in WhiteListObjects)
        {
            if (clickedObj == obj || clickedObj.transform.IsChildOf(obj.transform))
            {
                isWhitelistWorld = true;
                break;
            }
        }

        if (!isWhitelistWorld)
        {
            // Objek bukan whitelist dunia → error umum
            Debug.LogWarning("Klik objek tidak valid: " + clickedObj.name);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_5", 1);
            return;
        }

        // ================= 3. Cek apakah objek whitelist ada progress is_main =================
        foreach (string namaProgress in namaProgressList)
        {
            bool progressAktif = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);

            if (namaObjek.Contains("objek_01"))
            {
                if (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_5_01_makan_dan_minum"))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_01", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_01, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }

            if (namaObjek.Contains("objek_02"))
            {
                if (progressAktif && levelData.GetIsMainByLevelAndProgress(nomorLevel, "level_5_02_mencuci_piring"))
                {
                    Debug.Log($"Progress {namaProgress} valid di level {nomorLevel}");
                    return; // valid
                }
                else if (!isAddError)
                {
                    levelData.AddErrorToDatabase(id_progress, "salah_urutan_objek_02", 1);
                    isAddError = true;
                    Debug.LogWarning("Salah Urutan Objek_02, kurangi koin " + nomorLevel);
                    managerAudio?.PlayVAError();
                    KurangiKoin(2);
                    return;
                }
            }
        }

        // ================= 4. Kalau whitelist tapi tidak ada progress aktif =================
        if (!isAddError)
        {
            Debug.LogWarning($"[OnClickError] Tidak ada progress cocok di level {nomorLevel} untuk objek {namaObjek}");
            levelData.AddErrorToDatabase(id_progress, "klik_salah_level_5", 1);
            managerAudio?.PlayVAError2();
            KurangiKoin(1);
        }
    }

    private enum UICheckResult
    {
        None,       // tidak kena UI apapun
        Whitelist,  // kena UI whitelist
        NonWhitelist // kena UI tapi bukan whitelist
    }

    private UICheckResult CheckUI(Vector2 screenPos)
    {
        // Raycast ke semua UI di posisi klik
        var results = new List<RaycastResult>();
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPos
        };
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var go = result.gameObject;

            // Jika ada komponen whitelist → abaikan
            if (go.GetComponent<UIWhitelist>() != null)
                return UICheckResult.Whitelist;

            // Kalau bukan whitelist → NonWhitelist
            return UICheckResult.NonWhitelist;
        }

        return UICheckResult.None; // Tidak ada UI kena
    }

   // Add UI Whitelist
    void AddUIWhitelist()
    {
        foreach (var obj in WhiteListUI)
        {
            if (obj == null) continue;

            // ambil semua transform dari obj + child-childnya
            foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
            {
                if (t == null) continue;

                // cek apakah sudah ada komponen UIWhitelist
                if (t.gameObject.GetComponent<UIWhitelist>() == null)
                {
                    t.gameObject.AddComponent<UIWhitelist>();
                    Debug.Log($"UIWhitelist ditambahkan ke {t.gameObject.name}");
                }
                else
                {
                    Debug.Log($"{t.gameObject.name} sudah ada UIWhitelist");
                }
            }
        }
    }

    private bool IsAnyModalOpen()
    {
        if (ModalRoots == null) return false;
        for (int i = 0; i < ModalRoots.Count; i++)
        {
            var go = ModalRoots[i];
            if (go && go.activeInHierarchy) return true;
        }
        return false;
    }


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
