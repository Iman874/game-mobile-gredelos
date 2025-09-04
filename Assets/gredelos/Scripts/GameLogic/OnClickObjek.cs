using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class OnClickObjek : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    LevelDataController levelData;
    [Header("Logic Gameplay Controller")]
    public GameObject ManagerGameplay; // referensi ke ManagerGameplay (LevelHandController)
    private SpriteRenderer sr;
    [Header("Hover Effect")]
    public Color hoverColor = Color.yellow; // warna bersinar saat hover
    public float hoverIntensity = 0.5f;     // seberapa terang
    private Color originalColor;

    [Header("Level Info")]
    public int nomorGameplay = 1; // Nomor gameplay
    public int nomorLevel = 1;
    public string namaProgress = "lvl_X_0X_nama_progress";

    [Header("Jenis Kelamin Opsional")]
    public string jenisKelamin = ""; // default jenis kelamin

    void Awake()
    {
        levelData = LevelDataController.I;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        Debug.Log("Script aktif di: " + gameObject.name);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Objek diklik: " + gameObject.name);

        ManagerAudio.instance.PlaySFXClick();

        // Gameplay yang bisa dimulai hanya yang is_main == true
        // Jika nomorGameplay == 101, maka progress harus true
        bool progress = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);
        if (!progress && nomorGameplay != 101)
        {
            Debug.LogWarning($"Progress '{namaProgress}' pada level {nomorLevel} tidak dapat dimulai karena is_main == false.");
            return;
        }

        // Cari controller yang aktif di scene sesuai level
        if (nomorLevel == 1)
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel1>();
            if (controller != null)
                controller.OnClickObjek(nomorGameplay, namaProgress);
        }
        else if (nomorLevel == 3)
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel3>();
            if (controller != null)
            {
                controller.OnClickObjek(nomorGameplay, jenisKelamin, namaProgress);
            }
        }
        else if (nomorLevel == 4)
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel4>();
            if (controller != null)
            {
                controller.OnClickObjek(nomorGameplay,namaProgress);
            }
        } 
        else if (nomorLevel == 4 && nomorGameplay == 101 && namaProgress == "lanjutan")
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel4>();
            if (controller != null)
            {
                controller.OnClickObjek(nomorGameplay, namaProgress);
            }
        }
        else if (nomorLevel == 5 && nomorGameplay == 11)
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel5>();
            if (controller != null)
            {
                controller.OnClickObjek(nomorGameplay, namaProgress);
            }
        }
        else if (nomorLevel == 5 && nomorGameplay == 12)
        {
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel5>();
            if (controller != null)
            {
                controller.OnClickObjek(nomorGameplay, namaProgress);
            }
        }

        // Stop semua animation tangan
        var handController = FindFirstObjectByType<LevelHandController>();
        if(handController != null)
            handController.StopAllCoroutines();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover: tambah cahaya/sinar
        sr.color = originalColor * (1f + hoverIntensity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Kembalikan warna asli
        sr.color = originalColor;
    }
}
