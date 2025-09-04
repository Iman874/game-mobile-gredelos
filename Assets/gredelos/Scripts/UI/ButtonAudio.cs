using UnityEngine;

public class ButtonAudio : MonoBehaviour
{

    // Fungsi ini bisa di-assign ke OnClick Button
    public void PlayVAMenuLevel()
    {
        // Pastikan instance ManagerAudio ada, jika belum load otomatis
        ManagerAudio instanceAudio = ManagerAudio.GetInstance();
        if (instanceAudio != null)
        {
            instanceAudio.PlayVAMenuLevel();
        }
        else
        {
            Debug.LogWarning("ManagerAudio tidak ditemukan!");
        }
    }

    // Button Click SFX
    public void PlayButtonClickSFX()
    {
        // Pastikan instance ManagerAudio ada, jika belum load otomatis
        ManagerAudio instanceAudio = ManagerAudio.GetInstance();
        if (instanceAudio != null)
        {
            instanceAudio.PlaySFXButtonClick();
        }
        else
        {
            Debug.LogWarning("ManagerAudio tidak ditemukan!");
        }
    }

    // Cek static instance ManagerAudio
    public void CekScaneAktif(int nomorLevel)
    {
        ManagerAudio instanceAudio = ManagerAudio.GetInstance();
        if (instanceAudio != null)
        {
            instanceAudio.CekScaneAktif(nomorLevel);
        }
        else
        {
            Debug.LogWarning("ManagerAudio tidak ditemukan!");
        }
    }

    public void PlayVAEnding()
    {
        ManagerAudio instanceAudio = ManagerAudio.GetInstance();
        if (instanceAudio != null)
        {
            instanceAudio.PlayVAEnding();
        }
        else
        {
            Debug.LogWarning("ManagerAudio tidak ditemukan!");
        }
    }
}
