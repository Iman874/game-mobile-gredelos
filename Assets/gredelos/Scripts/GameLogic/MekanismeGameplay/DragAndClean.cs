using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class DragAndClean : MonoBehaviour
{
    [Header("Level Info")]
    public int level = 4;
    public int nomorGameplay = 10;

    [Header("Drag and Clean Settings")]
    public List<GameObject> objectsToClean;  // daftar collider target
    public int jumlahClean = 100;              // berapa kali bersentuhan sampai opacity 0

    private bool isDragging = false;
    private Vector3 startPosition;

    // hit count per object
    private Dictionary<GameObject, int> hitCounts = new Dictionary<GameObject, int>();

    // catat object yang sudah kena di frame ini untuk menghindari double count
    private HashSet<GameObject> frameHits = new HashSet<GameObject>();

     // --- tambahkan controller sebagai field ---
    private ControllerPlayObjekLevel4 controller;

    void Start()
    {
        startPosition = transform.position;

        foreach (var obj in objectsToClean)
        {
            if (obj != null && !hitCounts.ContainsKey(obj))
                hitCounts[obj] = 0;
        }

        // Cari Controller di scane berdasarkan level
        if (level == 4)
        {
            controller = FindAnyObjectByType<ControllerPlayObjekLevel4>();
            if (controller == null)
                Debug.LogError("ControllerPlayObjekLevel4 tidak ditemukan di scene.");
        }
    }

    void Update()
    {
        frameHits.Clear(); // reset setiap frame

        // PC input
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasPressedThisFrame)
                StartDrag(mousePos);
            else if (isDragging && Mouse.current.leftButton.isPressed)
                Drag(mousePos);
            else if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
                EndDrag();
        }

        // Touch input
        if (Touchscreen.current != null && Touchscreen.current.touches.Count > 0)
        {
            var touch = Touchscreen.current.touches[0];
            Vector2 touchPos = touch.position.ReadValue();

            switch (touch.phase.ReadValue())
            {
                case UnityEngine.InputSystem.TouchPhase.Began:
                    StartDrag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Moved:
                    if (isDragging) Drag(touchPos);
                    break;
                case UnityEngine.InputSystem.TouchPhase.Ended:
                case UnityEngine.InputSystem.TouchPhase.Canceled:
                    if (isDragging) EndDrag();
                    break;
            }
        }
    }

    private void StartDrag(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && hit.transform == transform)
            isDragging = true;
    }

    private void Drag(Vector2 screenPosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        worldPos.z = 0;
        transform.position = worldPos;

        // cek sentuhan dengan target
        foreach (var obj in objectsToClean)
        {
            if (obj == null || frameHits.Contains(obj)) continue;

            Collider2D targetCol = obj.GetComponent<Collider2D>();
            Collider2D myCol = GetComponent<Collider2D>();

            if (targetCol != null && myCol != null && myCol.IsTouching(targetCol))
            {
                frameHits.Add(obj); // pastikan cuma 1 hit per frame

                // update hitCount
                if (hitCounts[obj] < jumlahClean)
                {
                    hitCounts[obj]++;
                    UpdateOpacity(obj);

                    // jika sudah bersih, nonaktifkan object
                    if (hitCounts[obj] >= jumlahClean)
                        obj.SetActive(false);
                }
            }
        }
        // --- Debug: cek apakah semua objek sudah dinonaktifkan ---
        bool allCleaned = true;
        foreach (var obj in objectsToClean)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                allCleaned = false;
                break;
            }
        }

        if (allCleaned)
        {
            // Cari controller berdasarkan level
            if (level == 4)
            {
                if (controller != null)
                {
                    // Lakukan sesuatu dengan controller
                    controller.GetComponent<ControllerPlayObjekLevel4>()?.OnProgress(nomorGameplay, level, "");
                }
                else
                {
                    Debug.LogWarning("ControllerPlayObjekLevel4 tidak ditemukan di scene.");
                }
            }
        }
    }

    public void EndDrag()
    {
        isDragging = false;
        transform.position = startPosition;
    }

    private void UpdateOpacity(GameObject obj)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            float alpha = Mathf.Clamp01(1f - ((float)hitCounts[obj] / jumlahClean));
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }
    }
}
