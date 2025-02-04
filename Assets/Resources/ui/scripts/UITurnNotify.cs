using TMPro;
using System;
using UnityEngine;

public class UITurnNotify : MonoBehaviour
{
    public static UITurnNotify instance;
    [SerializeField] GameObject container;
    [SerializeField] TextMeshProUGUI txtMessage;
    bool isShow;
    float time = 0;
    Action callback;
    void Awake()
    {
        instance = this;
    }
    void Update()
    {
        if (isShow)
        {
            time += Time.deltaTime;
            if(time > 2)
            {
                Hide();
                if (callback != null)
                    callback();
            }
        }
    }
    public void Show(string message ,Action callback = null)
    {
        container.SetActive(true);
        txtMessage.text = message;
        isShow = true;
        time = 0;
        this.callback = callback;
    }
    public void Hide()
    {
        container.SetActive(false);
        isShow = false;
    }
}
