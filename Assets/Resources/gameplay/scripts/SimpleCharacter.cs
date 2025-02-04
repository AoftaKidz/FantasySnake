using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SimpleCharacter : MonoBehaviour
{
    public enum eDirection
    {
        Up = 0,
        Down,
        Left,
        Right
    }
    public enum eCharacterTurnState
    {
        None = 0,
        Move,
        Action,
        End
    }
    [SerializeField] List<SpriteRenderer> sprites;
    public Vector2Int tilePosition = Vector2Int.zero;
    public Vector2Int lastTilePosition = Vector2Int.zero;
    eCharacterTurnState state = eCharacterTurnState.None;
    eDirection direction = eDirection.Down;
    eDirection lastDirection = eDirection.Down;
    public GameObject tail = null;
    public bool isLeader = false;

    void Start()
    {
        transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);

        if (isLeader)
        {
            StartTurn();
        }
        else
        {
            state = eCharacterTurnState.None;
        }
    }
    public void StartTurn()
    {
        lastTilePosition = tilePosition;
        lastDirection = direction;
        state = eCharacterTurnState.Move;
        PathHighlight path = GetComponent<PathHighlight>();
        //path.Show();
        HandleDirection();
        //Blink
        Blink();
    }
    public void EndTurn()
    {
        state = eCharacterTurnState.None;

        StartTurn();

    }
    private void Update()
    {
        switch (state)
        {
            case eCharacterTurnState.Move:
                {

                    break;
                }
            case eCharacterTurnState.Action:
                {
                    break;
                }
            case eCharacterTurnState.End:
                {
                    break;
                }
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isLeader) return;

        if (context.performed)
        {
            var v = context.ReadValue<Vector2>();
            if (v.x >= 1)
                MoveRight();
            else if (v.x <= -1)
                MoveLeft();
            else if(v.y >= 1)
                MoveUp();
            else if(v.y <= -1)
                MoveDown();
        }

    }
    public void OnAction(InputAction.CallbackContext context)
    {
        if (!isLeader) return;

        if (context.performed)
        {
            if (state == eCharacterTurnState.Move && (tilePosition != lastTilePosition))
            {
                lastDirection = direction;
                lastTilePosition = tilePosition;
                state = eCharacterTurnState.Action;
                PathHighlight path = GetComponent<PathHighlight>();
                path.Hide();

                StopBlink();

                //Check collide collectable
                CheckCollideCollectableHero();
            }
            else if (state == eCharacterTurnState.Action)
            {
                EndTurn();
            }
        }
    }
    public void OnBack(InputAction.CallbackContext context)
    {
        if (!isLeader) return;

        if (context.performed)
        {
            if (state != eCharacterTurnState.Move) return;

            tilePosition = lastTilePosition;
            direction = lastDirection;
            transform.position = Global.ConvertTileToPosision(lastTilePosition.x, lastTilePosition.y);
            HandleDirection();
            RestoreTailPosition();
        }
    }
    public void MoveUp()
    {
        if (state != eCharacterTurnState.Move || (tilePosition != lastTilePosition)) return;
        lastTilePosition = tilePosition;
        lastDirection = direction;
        tilePosition.y++;
        transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);
        direction = eDirection.Up;
        HandleDirection();
        UpdateTailPosition();
    }
    public void MoveDown()
    {
        //Debug.Log("Down");
        //tilePosition.y--;
        //transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);
    }
    public void MoveLeft()
    {
        if (state != eCharacterTurnState.Move || (tilePosition != lastTilePosition)) return;
        lastTilePosition = tilePosition;
        lastDirection = direction;
        tilePosition.x--;
        transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);
        direction = eDirection.Left;
        HandleDirection();
        UpdateTailPosition();
    }
    public void MoveRight()
    {
        if (state != eCharacterTurnState.Move || (tilePosition != lastTilePosition)) return;
        lastTilePosition = tilePosition;
        lastDirection = direction;
        tilePosition.x++;
        transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);
        direction = eDirection.Right;
        HandleDirection();
        UpdateTailPosition();
    }
    void HandleDirection()
    {
        foreach (var sp in sprites)
        {
            sp.gameObject.SetActive(false);
        }
        sprites[(int)direction].gameObject.SetActive(true);
    }
    void Blink()
    {
        foreach (var sp in sprites)
        {
            var bl = sp.gameObject.GetComponent<SpriteBlink>();
            if(bl != null)
                bl.Blink();
        }
    }
    private void StopBlink()
    {
        foreach (var sp in sprites)
        {
            var bl = sp.gameObject.GetComponent<SpriteBlink>();
            if (bl != null)
                bl.StopBlink();
        }
    }
    public void SetDirection(eDirection dir)
    {
        direction = dir;
        HandleDirection();
    }
    public eDirection GetDirection()
    {
        return direction;
    }
    public void AddNewHero(GameObject hero)
    {
        if (tail == null)
        {
            tail = hero;
            SetTail(hero, direction);
            return;
        }
        else
        {
            GameObject t = tail;
            while (true)
            {
                if(t.GetComponent<SimpleCharacter>().tail == null)
                {
                    t.GetComponent<SimpleCharacter>().tail = hero;
                    t.GetComponent<SimpleCharacter>().SetTail(hero, t.GetComponent<SimpleCharacter>().GetDirection());
                    return;
                }
                else
                {
                    t = t.GetComponent<SimpleCharacter>().tail;
                }
            }
        }
    }
    void SetTail(GameObject hero, eDirection dir) 
    {
        if (!hero) return;

        var chr = hero.GetComponent<SimpleCharacter>();
        switch (dir)
        {
            case eDirection.Up:
                {
                    chr.tilePosition = new Vector2Int(tilePosition.x,tilePosition.y - 1);
                    chr.transform.position = Global.ConvertTileToPosision(chr.tilePosition.x, chr.tilePosition.y);
                    chr.SetDirection(dir);
                    break;
                }
            case eDirection.Down:
                {
                    chr.tilePosition = new Vector2Int(tilePosition.x, tilePosition.y + 1);
                    chr.transform.position = Global.ConvertTileToPosision(chr.tilePosition.x, chr.tilePosition.y);
                    chr.SetDirection(dir);
                    break;
                }
            case eDirection.Left:
                {
                    chr.tilePosition = new Vector2Int(tilePosition.x + 1, tilePosition.y);
                    chr.transform.position = Global.ConvertTileToPosision(chr.tilePosition.x, chr.tilePosition.y);
                    chr.SetDirection(dir);
                    break;
                }
            case eDirection.Right:
                {
                    chr.tilePosition = new Vector2Int(tilePosition.x - 1, tilePosition.y);
                    chr.transform.position = Global.ConvertTileToPosision(chr.tilePosition.x, chr.tilePosition.y);
                    chr.SetDirection(dir);
                    break;
                }
        }
    }
    void CheckCollideCollectableHero()
    {
        var heroes = GameObject.FindGameObjectsWithTag("Player");
        foreach (var hero in heroes)
        {
            var chr = hero.GetComponent<SimpleCharacter>();
            if (chr == null) continue;
            if(chr.isLeader) continue;

            if(chr.tilePosition == tilePosition)
            {
                AddNewHero(hero);
                break;
            }
        }
    }
    void UpdateTailPosition()
    {
        if (tail == null) return;

        GameObject t = gameObject;
        while (true)
        {
            if (t.GetComponent<SimpleCharacter>().tail == null)
            {
                return;
            }
            else
            {
                GameObject _parent = t;
                t = t.GetComponent<SimpleCharacter>().tail;
                t.GetComponent<SimpleCharacter>().lastDirection = t.GetComponent<SimpleCharacter>().direction;
                t.GetComponent<SimpleCharacter>().lastTilePosition = t.GetComponent<SimpleCharacter>().tilePosition;
                t.GetComponent<SimpleCharacter>().tilePosition = _parent.GetComponent<SimpleCharacter>().lastTilePosition;
                t.GetComponent<SimpleCharacter>().SetDirection(_parent.GetComponent<SimpleCharacter>().lastDirection);
                t.transform.position = Global.ConvertTileToPosision(t.GetComponent<SimpleCharacter>().tilePosition.x, t.GetComponent<SimpleCharacter>().tilePosition.y);
            }
        }
    }
    void RestoreTailPosition()
    {
        if (tail == null) return;

        GameObject t = gameObject;
        while (true)
        {
            if (t.GetComponent<SimpleCharacter>().tail == null)
            {
                return;
            }
            else
            {
                GameObject _parent = t;
                t = t.GetComponent<SimpleCharacter>().tail;
                t.GetComponent<SimpleCharacter>().direction = t.GetComponent<SimpleCharacter>().lastDirection;
                t.GetComponent<SimpleCharacter>().tilePosition = t.GetComponent<SimpleCharacter>().lastTilePosition;
                t.GetComponent<SimpleCharacter>().SetDirection(t.GetComponent<SimpleCharacter>().direction);
                t.transform.position = Global.ConvertTileToPosision(t.GetComponent<SimpleCharacter>().tilePosition.x, t.GetComponent<SimpleCharacter>().tilePosition.y);
            }
        }
    }
}
