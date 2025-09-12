using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelHandController : MonoBehaviour
{
    [Header("Animasi Awal (Sebelum Gameplay)")]
    [Tooltip("Durasi animasi dalam detik")]
    public int DurasiAnimasi = 3;
    [Tooltip("Jeda antar animasi dalam detik")]
    public int JedaAnimasi = 15;
    [Tooltip("Nomor Level")]
    public int Level;
    public List<GameObject> HandObjek; // objek-objek hand per objek
    public static LevelDataController levelData;
    public List<Progress> ProgressLevel = new();
    private List<Coroutine> runningCoroutines = new();

    // Database 
    private DbRoot db;
    private string FilePath => Path.Combine(Application.persistentDataPath, "game_data.json");

    void Awake()
    {
        levelData = LevelDataController.I;
        ProgressLevel = levelData.GetProgressDataByLevel(Level);

        foreach (var obj in HandObjek)
        {
            var cg = obj.GetComponent<CanvasGroup>();
            if (cg == null) cg = obj.AddComponent<CanvasGroup>();
            cg.alpha = 0f;       // alpha mulai dari 0
        }
    }

    // Hanya untuk testing di editor
    /*void Start()
    {
        MainkanSemuaAnimasi(); 
    }
    */

    public void StartWithJeda(int jedaFirst)
    {
        StartCoroutine(StartWithDelayCoroutine(jedaFirst));
    }

    private IEnumerator StartWithDelayCoroutine(int jedaFirst)
    {
        HideHand();
        yield return new WaitForSeconds(jedaFirst);
        MainkanSemuaAnimasi();
    }

    private void MainkanSemuaAnimasi()
    {
        // Stop semua coroutine lama dulu
        foreach (var co in runningCoroutines)
        {
            if (co != null) StopCoroutine(co);
        }
        runningCoroutines.Clear();

        // Cari index progress yang aktif (is_main == true)
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());

        // Loop semua HandObjek
        for (int i = 0; i < HandObjek.Count; i++)
        {
            var canvas = HandObjek[i].GetComponent<CanvasGroup>();
            if (canvas != null) canvas.alpha = 0f; // mulai dari invisible

            if (i == indexAktif)
            {
                // Aktifkan hanya objek yang sesuai progress aktif
                HandObjek[i].SetActive(true);

                // Mainkan animasi
                var co = StartCoroutine(LoopAnimasiHand(HandObjek[i]));
                runningCoroutines.Add(co);
            }
            else
            {
                // Nonaktifkan sisanya
                HandObjek[i].SetActive(false);
            }
        }
    }

    private IEnumerator LoopAnimasiHand(GameObject hand)
    {
        var pointer = hand.GetComponent<PointerAnimation>();
        var sr = hand.GetComponent<SpriteRenderer>();

        if (sr == null)
        {
            Debug.LogWarning("Hand object tidak memiliki SpriteRenderer: " + hand.name);
            yield break;
        }

        while (true)
        {
            if (!hand.activeInHierarchy) yield break;
            // Fade in
            HideObjectAlpha(hand);

            yield return FadeSprite(sr, 0f, 1f, 0.5f);

            // Mainkan animasi pointer tapi tetap bisa fade
            if (pointer != null)
                pointer.PlayAnimation(hand.GetComponent<RectTransform>(), DurasiAnimasi);

            // Tunggu animasi pointer selesai
            float elapsed = 0f;
            while (elapsed < DurasiAnimasi)
            {
                elapsed += Time.deltaTime;
                yield return null; // tetap update setiap frame
            }

            // Fade out
            yield return FadeSprite(sr, 1f, 0f, 0.5f);
            HideObjectAlpha(hand);

            // Tunggu jeda tapi tetap invisible
            float jedaElapsed = 0f;
            while (jedaElapsed < JedaAnimasi)
            {
                jedaElapsed += Time.deltaTime;
                yield return null; // hand tetap invisible selama jeda
            }
        }
    }

     private void HideObjectAlpha(GameObject obj)
    {
        if (obj == null) return;

        // jangan pakai obj.SetActive(false);

        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            var c = sr.color;
            c.a = 0f;
            sr.color = c;
        }

        var cg = obj.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 0f;
    }

    // Fungsi fade untuk SpriteRenderer
    private IEnumerator FadeSprite(SpriteRenderer sr, float from, float to, float duration)
    {
        float t = 0f;
        Color c = sr.color;
        c.a = from;
        sr.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = to;
        sr.color = c;
    }


    // Fungsi helper untuk menghilangkan hand saat jeda
    // Fungsi untuk langsung menyembunyikan semua hand
    public void HideHand()
    {
        // Stop semua coroutine animasi yang lagi jalan
        foreach (var co in runningCoroutines)
        {
            if (co != null) StopCoroutine(co);
        }
        runningCoroutines.Clear();

        // Sembunyikan semua handObjek
        foreach (var obj in HandObjek)
        {
            if (obj != null)
            {
                obj.SetActive(false);

                // Kalau ada SpriteRenderer, pastikan alpha = 0
                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    var c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }

                // Kalau ada CanvasGroup, pastikan alpha = 0
                var cg = obj.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Load Data")]
    public void LoadDataLevelEditor()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            db = JsonUtility.FromJson<DbRoot>(json);
            Debug.Log("Data level berhasil di-load di editor.");

            string playerName = db.player[0].nama_player;
            string levelId = $"level_{Level}_{playerName}";

            if (db.progress != null)
            {
                ProgressLevel = db.progress
                                  .Where(x => x.fk_id_level == levelId)
                                  .ToList();

                Debug.Log($"[EDITOR] Progress untuk level {Level} ({levelId}) berhasil di-load. Total: {ProgressLevel.Count}");
            }
            else
            {
                ProgressLevel = new List<Progress>();
                Debug.LogWarning("[EDITOR] Database progress kosong atau null.");
            }
        }
    }
#endif
}
