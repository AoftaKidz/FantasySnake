using UnityEngine;

public class SpriteBlink : MonoBehaviour
{
    float opacity = 0;
    bool isBlink;
    bool isStart;
    float speed = 10;
    SpriteRenderer spriteRenderer;
    public bool autoBlink = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        if (autoBlink)
            Blink();
    }
    void Update()
    {
        if (!isStart) return;
        if (spriteRenderer == null) return;

        if(isBlink )
        {
            opacity += Time.deltaTime * speed;
            if(opacity >=1 )
            {
                isBlink = false;
                opacity = 1;
            }
        }
        else
        {
            opacity -= Time.deltaTime * speed;
            if (opacity <= 0)
            {
                isBlink = true;
                opacity = 0;
            }
        }

        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }
    public void Blink()
    {
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        isStart = true;
        isBlink = false;
        opacity = 1;
        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }
    public void StopBlink()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        isStart = false;
        isBlink = false;
        opacity = 1;
        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }
}
