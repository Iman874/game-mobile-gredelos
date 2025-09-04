using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class OnClickObjekProgress : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer sr;
    [Header("Hover Effect")]
    public Color hoverColor = Color.yellow; 
    public float hoverIntensity = 0.5f;     
    private Color originalColor;

    [Header("Gameplay Info")]
    public int nomorGameplay = 111; // Nomor gameplay yang dipicu

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        Debug.Log("Script aktif di: " + gameObject.name);
    }
    

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Objek diklik (OnProgress khusus Level 5): " + gameObject.name);

        ManagerAudio.instance.PlaySFXClick();

        if (nomorGameplay == 111)
        {
            // Panggil fungsi OnProgress di ControllerPlayObjekLevel5
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel5>();
            if (controller != null)
            {
                controller.OnProgress(111, 5, "OnProgress_5_1");
            }
        }
        if (nomorGameplay == 121)
        {
            // Panggil fungsi OnProgress di ControllerPlayObjekLevel5
            var controller = FindFirstObjectByType<ControllerPlayObjekLevel5>();
            if (controller != null)
            {
                controller.OnProgress(121, 5, "OnProgress_5_2");
            }
        }

        // Stop semua animation tangan
        var handController = FindFirstObjectByType<LevelHandController>();
        if (handController != null)
            handController.StopAllCoroutines();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sr.color = originalColor * (1f + hoverIntensity);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sr.color = originalColor;
    }
}
