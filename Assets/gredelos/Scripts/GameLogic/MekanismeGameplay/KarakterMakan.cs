using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KarakterMakan : MonoBehaviour
{
    [Header("Config Makan")]
    public int porsiMakan = 3; // total berapa kali makan
    public float fps = 12f;    // kecepatan animasi
    public List<Sprite> spriteTahapMakan; // urutan sprite animasi makan

    private SpriteRenderer sr;
    private int makanCount = 0; // sudah makan berapa kali
    private bool sedangAnimasi = false;

    // cache biar gak panggil FindObjectOfType terus
    private ControllerPlayObjekLevel5 controller;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (spriteTahapMakan != null && spriteTahapMakan.Count > 0)
        {
            sr.sprite = spriteTahapMakan[0]; // mulai dari sprite awal
        }

        // cari controller sekali saja di awal
        controller = FindObjectOfType<ControllerPlayObjekLevel5>();
        if (controller != null)
        {
            Debug.Log("ControllerPlayObjekLevel5 ditemukan di Start.");
        }
        else
        {
            Debug.LogWarning("ControllerPlayObjekLevel5 tidak ditemukan di Start.");
        }
    }

    public void Makan()
    {
        if (sedangAnimasi) return; // biar gak double kalau dipanggil cepat

        if (makanCount < porsiMakan)
        {
            makanCount++;
            StartCoroutine(PutarAnimasiMakan());
        }
        else
        {
            Debug.Log("Karakter sudah kenyang (dipanggil dari Makan).");

            if (controller != null)
            {
                try
                {
                    controller.OnSelesaiGameplay(11);
                    Debug.Log("Memanggil OnSelesaiGameplay(11) di ControllerPlayObjekLevel5.");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error saat memanggil OnSelesaiGameplay: " + ex.Message + "\n" + ex.StackTrace);
                }
            }
            else
            {
                Debug.LogWarning("ControllerPlayObjekLevel5 masih null saat karakter kenyang.");
            }
        }
    }

    private IEnumerator PutarAnimasiMakan()
    {
        sedangAnimasi = true;

        if (spriteTahapMakan != null && spriteTahapMakan.Count > 0)
        {
            float delay = 1f / fps;

            foreach (var sprite in spriteTahapMakan)
            {
                sr.sprite = sprite;
                yield return new WaitForSeconds(delay);
            }
        }

        Debug.Log($"Makan ke-{makanCount}/{porsiMakan}");

        if (makanCount >= porsiMakan)
        {
            Debug.Log("Karakter sudah kenyang (dari coroutine).");

            if (controller != null)
            {
                controller.OnSelesaiGameplay(11);
                Debug.Log("Coroutine: OnSelesaiGameplay dipanggil setelah kenyang.");
            }
        }

        sedangAnimasi = false;
    }
}
