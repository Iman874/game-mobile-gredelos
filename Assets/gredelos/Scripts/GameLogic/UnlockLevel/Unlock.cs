using UnityEngine;

public class Unlock : MonoBehaviour
{
    [Header("Nomor Level")]
    public int NomorLevel;

    private ControllerUnlockLevel controllerUnlockLevel;

    public void UnlockLevel()
    {
        // Cari ControllerUnlockLevel otomatis di scene
        controllerUnlockLevel = FindFirstObjectByType<ControllerUnlockLevel>();

        if (controllerUnlockLevel != null)
        {
            controllerUnlockLevel.ShowUnlockLevelWindow(NomorLevel);
        }
        else
        {
            Debug.LogError("ControllerUnlockLevel tidak ditemukan di scene!");
        }
    }
}
