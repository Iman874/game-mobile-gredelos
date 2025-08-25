using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class OnClickObjek : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    LevelDataController levelData;
    [Header("Logic Gameplay Controller")]
    public GameObject ControllerPlayObjek; // referensi ke ControllerPlayObjek
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
        bool progress = levelData.GetProgressIsMainByLevelAndProgress(nomorLevel, namaProgress);
        if (progress)
        {
            levelData.StartLevel_OnClick(nomorLevel, namaProgress);
            ControllerPlayObjek.GetComponent<ControllerPlayObjekClick>().OnClickObjek(nomorGameplay);
            // Nonaktifkan semua animation tangan di level
            if (ManagerGameplay.GetComponent<LevelHandController>() != null)
                ManagerGameplay.GetComponent<LevelHandController>().StopAllCoroutines();
        }
        else
        {
            Debug.LogWarning($"Progress '{namaProgress}' pada level {nomorLevel} tidak dapat dimulai karena is_main == false.");
            return;
        }
       
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
