using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("Level Info")]
    public int nomorLevel = 1;

    [Header("Controller Unlock Level")]
    public ControllerUnlockLevel ControllerUnlockLevel;

    private object controller; // bisa level1 atau level3

    void Start()
    {
        // cari otomatis controller sesuai level
        if (nomorLevel == 1)
            controller = FindFirstObjectByType<ControllerPlayObjekLevel1>();
        else if (nomorLevel == 3)
            controller = FindFirstObjectByType<ControllerPlayObjekLevel3>();
        else if (nomorLevel == 4)
            controller = FindFirstObjectByType<ControllerPlayObjekLevel4>();
        else if (nomorLevel == 5)
            controller = FindFirstObjectByType<ControllerPlayObjekLevel5>();

        if (controller == null)
            Debug.LogError($"Controller untuk level {nomorLevel} tidak ditemukan di scene!");
    }

    public void OnClickNextProgress()
    {
        if (controller is ControllerPlayObjekLevel1 c1)
            c1.NextProgress();
        else if (controller is ControllerPlayObjekLevel3 c3)
            c3.NextProgress();
        else if (controller is ControllerPlayObjekLevel4 c4)
            c4.NextProgress();
        else if (controller is ControllerPlayObjekLevel5 c5)
            c5.NextProgress();

        Debug.Log("Tombol Next Progress diklik.");
    }

    public void OnClickUlangiLevel()
    {
        if (controller is ControllerPlayObjekLevel1 c1)
            c1.UlangiLevel();
        else if (controller is ControllerPlayObjekLevel3 c3)
            c3.UlangiLevel();
        else if (controller is ControllerPlayObjekLevel4 c4)
            c4.UlangiLevel();
        else if (controller is ControllerPlayObjekLevel5 c5)
            c5.UlangiLevel();

        Debug.Log("Tombol Ulangi Level diklik.");
    }

    public void OnClickNextLevel()
    {
        if (controller is ControllerPlayObjekLevel1 c1)
            c1.NextLevel();
        else if (controller is ControllerPlayObjekLevel3 c3)
            c3.NextLevel();
        else if (controller is ControllerPlayObjekLevel4 c4)
            c4.NextLevel();
        else if (controller is ControllerPlayObjekLevel5 c5)
            c5.NextLevel();

        Debug.Log("Tombol Next Level diklik.");
    }

    public void OnClickOke()
    {
        ControllerUnlockLevel?.OnClickOke();
    }
}
