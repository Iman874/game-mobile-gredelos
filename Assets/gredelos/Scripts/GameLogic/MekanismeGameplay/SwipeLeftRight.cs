using UnityEngine;
using System.Collections.Generic;

public class SwipeLeftRight : MonoBehaviour
{
    [Header("Game Controller")]
    public int nomorLevel;
    public int nomorGameplay;

    [Header("Info Swipe Objek")]
    public GameObject swipeObject;
    public int totalSwipes = 2; // kanan ke tengah + kiri ke tengah
    public Sprite[] steps;

    private void Start()
    {
        // Inisialisasi atau pengaturan awal jika diperlukan
    }

    private void Update()
    {
        // Logika pembaruan setiap frame jika diperlukan
    }
}