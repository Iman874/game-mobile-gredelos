using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Controller Play Objek")]
    public GameObject ControllerPlayObjek; // referensi ke ControllerPlayObjek

    public GameObject ControllerUnlockLevel; // referensi ke ControllerUnlockLevel

    // Fungsi untuk tombol next
    public void OnClickNextProgress()
    {
        // Panggil fungsi pada ControllerPlayObjekLevel1 untuk melanjutkan ke level progress berikutnya
        ControllerPlayObjek.GetComponent<ControllerPlayObjekLevel1>().NextProgress();

        // Debug
        Debug.Log("Tombol Next Progress diklik, lanjut ke progress berikutnya.");
    }

    // Fungsi untuk tombol ulangi
    public void OnClickUlangiLevel()
    {
        // Panggil fungsi pada ControllerPlayObjekLevel1 untuk mengulangi level saat ini
        ControllerPlayObjek.GetComponent<ControllerPlayObjekLevel1>().UlangiLevel();

        // Debug
        Debug.Log("Tombol Ulangi level diklik, ulangi level saat ini.");
    }

    // Fungsi untuk ke level selanjutnya
    public void OnClickNextLevel()
    {
        // Panggil fungsi pada ControllerPlayObjekLevel1 untuk melanjutkan ke level berikutnya
        ControllerPlayObjek.GetComponent<ControllerPlayObjekLevel1>().NextLevel();

        // Debug
        Debug.Log("Tombol Next Level diklik, lanjut ke level berikutnya.");
    }

    // Fungsi pada button Oke
    public void OnClickOke()
    {
        // Panggill fungsi pada Conttroller Unlock Level
        ControllerUnlockLevel.GetComponent<ControllerUnlockLevel>().OnClickOke();
    }
}
