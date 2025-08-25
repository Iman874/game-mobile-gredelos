using UnityEngine;

public class ButtonUIPilihKarakter : MonoBehaviour
{
    // Ambil scane dari inspector
    [Header("Scene Menu Utama")]
    public string sceneNameback;

    // Fungsi untuk tombol back
    public void BackToMenuUtama()
    {
        // Load scene menu utama
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneNameback);
    }
}
