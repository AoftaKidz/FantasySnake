using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UITurnbase : MonoBehaviour
{
    public static UITurnbase instance;
    [SerializeField] GameObject container;
    [SerializeField] List<GameObject> buttons;
    int indexMenu = 0;
    bool isShow = false;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Hide();
    }
    public bool IsAppear()
    {
        return isShow;
    }
    public void Show()
    {
        isShow = true;
        container.SetActive(true);
        indexMenu = 0;
        SetButtons();
    }
    public void Hide()
    {
        isShow = false;
        container.SetActive(false);
    }
    void SetButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            if(i == indexMenu)
            {
                buttons[i].GetComponentInChildren<BlinkText>().Blink();
            }
            else
            {
                buttons[i].GetComponentInChildren<BlinkText>().StopBlink();
            }
        }
    }
    public void OnPrev()
    {
        indexMenu--;
        if (indexMenu < 0)
            indexMenu = 2;
        SetButtons();
    }
    public void OnNext()
    {
        indexMenu++;
        if (indexMenu > 2)
            indexMenu = 0;
        SetButtons();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isShow) return;

        if (context.performed)
        {
            var v = context.ReadValue<Vector2>();
           /* if (v.x >= 1)
                MoveRight();
            else if (v.x <= -1)
                MoveLeft();
            else if (v.y >= 1)
                MoveUp();
            else if (v.y <= -1)
                MoveDown();*/
           if(v.y >= 1)
                OnPrev();
           else if(v.y <= -1)
                OnNext();
        }

    }
    public void OnAction(InputAction.CallbackContext context)
    {
        if (!isShow) return;

        if (context.performed)
        {
           if(indexMenu == 0)
            {
                //Move
                if (!CharacterTurnbase.instance.isAlreadyMove)
                {
                    CharacterTurnbase.instance.StartMove();
                    Hide();
                }
                
            }else if (indexMenu == 1)
            {
                //Attack
                //var player = GameObject.FindGameObjectWithTag("Player");
                //player.GetComponent<CharacterTurnbase>().StartMove();
                if (!CharacterTurnbase.instance.isAlreadyAttack)
                {
                    CharacterTurnbase.instance.AttackTurn();
                    Hide();
                }
            }
            else
            {
                //var player = GameObject.FindGameObjectWithTag("Player");
                //player.GetComponent<CharacterTurnbase>().StartMove();
                CharacterTurnbase.instance.EndTurn();
            }
        }
    }
    public void OnBack(InputAction.CallbackContext context)
    {
        if (!isShow) return;

        if (context.performed)
        {

        }
    }
}
