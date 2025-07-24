using UnityEngine;

public class fit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;
        Vector3 scale = transform.localScale;

        float scaleX = screenWidth / spriteSize.x;
        float scaleY = screenHeight / spriteSize.y;

        // Pakai yang lebih besar agar menutupi penuh
        float finalScale = Mathf.Max(scaleX, scaleY);

        transform.localScale = new Vector3(finalScale, finalScale, 1);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
