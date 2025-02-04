using UnityEngine;
using System.Collections.Generic;

public class CharacterDirection : MonoBehaviour
{
    public enum eDirection
    {
        Up = 0,
        Down,
        Left,
        Right
    }
    public eDirection direction = eDirection.Down;
    public eDirection lastDirection = eDirection.Down;
    [SerializeField] List<SpriteRenderer> sprites;
    private void Start()
    {
        lastDirection = direction;
        //RandomColor();
    }
    private void Update()
    {
        //HandleDirection();
    }
    public void HandleDirection()
    {
        foreach (var sp in sprites)
        {
            sp.gameObject.SetActive(false);
        }
        sprites[(int)direction].gameObject.SetActive(true);
    }
    public void Backup()
    {
        lastDirection = direction;
    }
    public void Restore()
    {
        direction = lastDirection;
        HandleDirection();
    }
    public void Blink()
    {
        foreach (var sp in sprites)
        {
            var bl = sp.gameObject.GetComponent<SpriteBlink>();
            if (bl != null)
                bl.Blink();
        }
    }
    public void StopBlink()
    {
        foreach (var sp in sprites)
        {
            var bl = sp.gameObject.GetComponent<SpriteBlink>();
            if (bl != null)
                bl.StopBlink();
        }
    }
    void RandomColor()
    {
        float r = Random.Range(0.0f, 255.0f) / 255f;
        float g = Random.Range(0.0f, 255.0f) / 255f;
        float b = Random.Range(0.0f, 255.0f) / 255f;
        Color c = new Color(r, g, b);
        foreach (var sp in sprites)
        {
            sp.color = c;
        }
    }
}
