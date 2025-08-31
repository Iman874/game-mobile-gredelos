using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class LevelHandController2 : MonoBehaviour
{
    [Header("Animasi Awal (Sebelum Gameplay)")]
    public int DurasiAnimasi = 3;
    public int JedaAnimasi = 15;
    public int Level;

    [Header("Hand Objects")]
    public GameObject HandObjekLaki;
    public GameObject HandObjekPerempuan;
    public GameObject HandObjekSisir;

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

        // Buat Semua Hand trasnparan
        HandObjekLaki.GetComponent<CanvasGroup>().alpha = 0f;
        HandObjekPerempuan.GetComponent<CanvasGroup>().alpha = 0f;
        HandObjekSisir.GetComponent<CanvasGroup>().alpha = 0f;
    }

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
        // Stop coroutine lama
        foreach (var co in runningCoroutines)
        {
            if (co != null) StopCoroutine(co);
        }
        runningCoroutines.Clear();

        // Cari index progress aktif
        int indexAktif = ProgressLevel.FindIndex(p => p.Get_is_main());
        Debug.Log($"Index Aktif = {indexAktif}");

        GameObject handToPlay = null;

        if (indexAktif == 0)
        {
            // Cek gender player
            if (levelData != null)
            {
                if (levelData.GetJenisKelamin() == "laki-laki")
                {
                    handToPlay = HandObjekLaki;
                    Debug.Log("Hand yang dimainkan: " + handToPlay.name);
                }
                else if (levelData.GetJenisKelamin() == "perempuan")
                {
                    handToPlay = HandObjekPerempuan;
                    Debug.Log("Hand yang dimainkan: " + handToPlay.name);
                }
            }
        }
        else if (indexAktif == 1)
        {
            handToPlay = HandObjekSisir;
        }
        else
        {
            Debug.Log("Tidak ada hand yang dimainkan karena indexAktif diluar jangkauan.");
        }

        // Mainkan animasi kalau ada hand terpilih
        if (handToPlay != null)
        {
            handToPlay.SetActive(true);
            var co = StartCoroutine(LoopAnimasiHand(handToPlay));
            runningCoroutines.Add(co);
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
            
            hand.SetActive(true);
            yield return FadeSprite(sr, 0f, 1f, 0.5f);

            if (pointer != null)
            {
                // Panggil function langsung, jangan StartCoroutine dari pointer
                pointer.PlayAnimation(hand.GetComponent<RectTransform>(), DurasiAnimasi);
            }

            float elapsed = 0f;
            while (elapsed < DurasiAnimasi)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return FadeSprite(sr, 1f, 0f, 0.5f);

            float jedaElapsed = 0f;
            while (jedaElapsed < JedaAnimasi)
            {
                jedaElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

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

    public void HideHand()
    {
        foreach (var co in runningCoroutines)
        {
            if (co != null) StopCoroutine(co);
        }
        runningCoroutines.Clear();

        // sembunyikan semua dengan alpha
        HideObjectAlpha(HandObjekLaki);
        HideObjectAlpha(HandObjekPerempuan);
        HideObjectAlpha(HandObjekSisir);
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
