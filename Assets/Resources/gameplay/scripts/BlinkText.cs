using UnityEngine;
using TMPro;

public class BlinkText : MonoBehaviour
{
    float opacity = 0;
    bool isBlink;
    bool isStart;
    float speed = 10;
    TextMeshProUGUI text;
    public bool autoBlink = false;
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        if (autoBlink)
            Blink();
    }
    void Update()
    {
        if (!isStart) return;
        if (text == null) return;

        if (isBlink)
        {
            opacity += Time.unscaledDeltaTime * speed;
            if (opacity >= 1)
            {
                isBlink = false;
                opacity = 1;
            }
        }
        else
        {
            opacity -= Time.unscaledDeltaTime * speed;
            if (opacity <= 0)
            {
                isBlink = true;
                opacity = 0;
            }
        }

        Color c = text.color;
        c.a = opacity;
        text.color = c;
    }
    public void Blink()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        isStart = true;
        isBlink = false;
        opacity = 1;
        Color c = text.color;
        c.a = opacity;
        text.color = c;
    }
    public void StopBlink()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
        isStart = false;
        isBlink = false;
        opacity = 1;
        Color c = text.color;
        c.a = opacity;
        text.color = c;
    }
}
