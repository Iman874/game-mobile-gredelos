using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    [Header("Audio")]
    public AudioMixer audioMixer; // drag AudioMixer di inspector

    private void Start()
    {
        // Load nilai dari PlayerPrefs biar setting tersimpan
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float volume)
    {
        // Volume di AudioMixer biasanya dalam dB, jadi pakai log10
        audioMixer.SetFloat("Volume", Mathf.Log10(volume) * 20);

        if (volumeText != null)
        {
            volumeText.text = Mathf.RoundToInt(volume * 100).ToString(); // Tampilkan sebagai persen
        }

        // Simpan biar tetap ada saat game dibuka lagi
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }
}
