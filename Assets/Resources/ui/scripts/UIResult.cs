using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIResult : MonoBehaviour
{
    public static UIResult instance = null;
    [SerializeField] GameObject container;
    [SerializeField] List<GameObject> buttons;
    int indexMenu = 0;
    bool isShow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        Hide();
    }
    public bool IsAppear()
    {
        return isShow;
    }
    public void Show()
    {
        container.SetActive(true);
        isShow = true;
        indexMenu = 0;
        SetButtons();
    }
    public void Hide()
    {
        container.SetActive(false);
        isShow = false;
    }
    public void OnPrev()
    {
        indexMenu--;
        if (indexMenu < 0)
            indexMenu = 1;
        SetButtons();
    }
    public void OnNext()
    {
        indexMenu++;
        if (indexMenu > 1)
            indexMenu = 0;
        SetButtons();
    }
    void SetButtons()
    {
        for(int i=0; i < buttons.Count;i++)
        {
            if(i == indexMenu)
                buttons[i].GetComponentInChildren<BlinkText>().Blink();
            else
                buttons[i].GetComponentInChildren<BlinkText>().StopBlink();
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isShow) return;

        if (context.performed)
        {
            var v = context.ReadValue<Vector2>();
            if (v.y >= 1)
                OnPrev();
            else if (v.y <= -1)
                OnNext();
        }

    }
    public void OnAction(InputAction.CallbackContext context)
    {
        if (!isShow) return;

        if (context.performed)
        {
            if (indexMenu == 0)
            {
                SceneManager.LoadScene(1);
            }
            else if (indexMenu == 1)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
