using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickObjek2 : MonoBehaviour, IPointerDownHandler
{
    [Header("Window Info")]
    public GameObject ShadowBackground;
    public GameObject SettingWindow;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Objek diklik: " + gameObject.name);

        // Tampilkan window setting + background
        if (ShadowBackground != null) ShadowBackground.SetActive(true);
        if (SettingWindow != null)
        {
            SettingWindow.SetActive(true);

            // Aktifkan animasi scale kalau ada
            var animScale = SettingWindow.GetComponent<AnimatorScale>();
            if (animScale != null)
                animScale.PlayShow();
        }

        // Tambahkan efek audio
        if (ManagerAudio.instance != null)
            ManagerAudio.instance.PlaySFXClick();
    }
}
