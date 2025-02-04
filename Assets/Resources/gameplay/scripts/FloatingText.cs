using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] TextMeshPro txtMessage;
    float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > 1)
        {
            Destroy(gameObject);
        }
        transform.position += Vector3.up * Time.deltaTime * 0.2f;
    }
    public void SetText(string text,Color c)
    {
        txtMessage.text = text;
        txtMessage.color = c;
        time = 0;
    }
}
