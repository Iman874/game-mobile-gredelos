using UnityEngine;

public class ButtonUnlock : MonoBehaviour
{
    [Header("Controller Unlock Level")]
    public ControllerUnlockLevel ControllerUnlockLevel;

    public void OnClickOke()
    {
        ControllerUnlockLevel?.OnClickOke();
    }
}
