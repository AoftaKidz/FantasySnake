using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public struct RotateCharacterData
{
    public Vector2Int tilePosition;
    public CharacterDirection.eDirection direction;
}
public class CharacterTurnbase : MonoBehaviour
{
    public static CharacterTurnbase instance;
    public enum eCharacterTurnState
    {
        None = 0,
        Move,
        Action,
        End
    }
    public eCharacterTurnState state = eCharacterTurnState.None;
    public List<GameObject> heroes = new List<GameObject>();
    public bool isAlreadyMove = false;
    public bool isAlreadyAttack = false;
    [SerializeField] GameObject tileSelection;

    private void Awake()
    {
        instance = this;
    }
    public void StartMove()
    {
        state = eCharacterTurnState.Move;
        PathHighlight path = GetComponent<PathHighlight>();
        path.Show(heroes[0].GetComponent<CharacterDirection>().direction, heroes[0].GetComponent<CharacterPosition>().tilePosition,true);
        
        heroes[0].GetComponent<CharacterDirection>().Blink();
        isAlreadyMove = false;
        tileSelection.SetActive(false);
    }
    public void AttackTurn()
    {
        state = eCharacterTurnState.Action;
        PathHighlight path = GetComponent<PathHighlight>();
        path.Show(heroes[0].GetComponent<CharacterDirection>().direction, heroes[0].GetComponent<CharacterPosition>().tilePosition,false);
        heroes[0].GetComponent<CharacterDirection>().Blink();
        isAlreadyAttack = false;
        tileSelection.SetActive(true);
        tileSelection.transform.position = heroes[0].transform.position;
    }
    public void EndTurn()
    {
        state = eCharacterTurnState.None;
        heroes[0].GetComponent<CharacterDirection>().StopBlink();
        PathHighlight path = GetComponent<PathHighlight>();
        path.Hide();

        UITurnbase.instance.Hide();
        TurnbaseManager.instance.StartTurn(TurnbaseManager.eTurnState.Enemy);
        isAlreadyMove = false;
        isAlreadyAttack=false;
        tileSelection.SetActive(false);

    }
    public void CancelTurn()
    {
        state=eCharacterTurnState.None;
        tileSelection.SetActive(false);
        UITurnbase.instance.Show();
        GetComponent<PathHighlight>().Hide();
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
    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RotateCharacterLeft();
        }
    }
    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RotateCharacterRight();
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (UIGamepause.instance.IsAppear()) return;

        if (context.performed)
        {
            var v = context.ReadValue<Vector2>();
            if (v.x >= 1)
                MoveRight();
            else if (v.x <= -1)
                MoveLeft();
            else if (v.y >= 1)
                MoveUp();
            else if (v.y <= -1)
                MoveDown();
        }

    }
    public void OnAction(InputAction.CallbackContext context)
    {
        if (UIGamepause.instance.IsAppear()) return;

        if (context.performed)
        {
            if (state == eCharacterTurnState.Move && (heroes[0].GetComponent<CharacterPosition>().tilePosition != heroes[0].GetComponent<CharacterPosition>().lastTilePosition))
            {
    
                state = eCharacterTurnState.None;
                PathHighlight path = GetComponent<PathHighlight>();
                path.Hide();

                heroes[0].GetComponent<CharacterDirection>().StopBlink();

                for (int i = 0;i < heroes.Count; i++)
                {
                    heroes[i].GetComponent<CharacterPosition>().Backup();
                    heroes[i].GetComponent<CharacterDirection>().Backup();
                }

                CheckCollideCollectableHero();
                UITurnbase.instance.Show();
                isAlreadyMove = true;

                //Check Item
                CheckCollideItem();
            }
            else if (state == eCharacterTurnState.Action)
            {
                //EndTurn();
                isAlreadyAttack = true;
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                foreach (var enemy in enemies)
                {
                    if(enemy.transform.position == tileSelection.transform.position)
                    {
                        heroes[0].GetComponent<CharacterAttack>().Attacking(enemy, () => {
                            CancelTurn();
                        });
                        return;
                    }
                }
            }
        }
    }
    public void OnBack(InputAction.CallbackContext context)
    {
        if (UIGamepause.instance.IsAppear()) return;

        if (context.performed)
        {
            if (state == eCharacterTurnState.None) return;
            {
                if(state == eCharacterTurnState.Move)
                {
                    if(isAlreadyMove)
                    {
                        RestoreMove();
                        isAlreadyMove=false;
                    }
                    else
                    {
                        CancelTurn();
                    }
                }
                else if (state == eCharacterTurnState.Action)
                {
                    if(isAlreadyAttack)
                    {
                        tileSelection.transform.position = heroes[0].transform.position;
                        isAlreadyAttack = false;
                    }
                    else
                    {
                        CancelTurn();
                    }
        
                }else if(state == eCharacterTurnState.End)
                {

                }
            }
        }
    }
    public void MoveUp()
    {
        if (heroes.Count > 1 && heroes[0].GetComponent<CharacterDirection>().direction == CharacterDirection.eDirection.Down) return;

        if(state == eCharacterTurnState.Move && !isAlreadyMove)
        {
            Vector2 tile = heroes[0].GetComponent<CharacterPosition>().tilePosition;//Global.ConvertPositionToTile(new Vector2(tileSelection.transform.position.x, tileSelection.transform.position.y));
            Vector2 p = Global.ConvertTileToPosision((int)tile.x, (int)tile.y + 1);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                HandleMoveByDirection(CharacterDirection.eDirection.Up);
                isAlreadyMove = true;
            }
        }
        else if(state == eCharacterTurnState.Action && !isAlreadyAttack)
        {
            Vector2 tile = Global.ConvertPositionToTile(new Vector2(tileSelection.transform.position.x, tileSelection.transform.position.y));
            Vector2 p = Global.ConvertTileToPosision((int)tile.x, (int)tile.y + 1);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                tileSelection.transform.position = p;
                isAlreadyAttack=true;
            }
        }
    }
    public void MoveDown()
    {
        if (heroes.Count > 1 && heroes[0].GetComponent<CharacterDirection>().direction == CharacterDirection.eDirection.Up) return;
        if (state == eCharacterTurnState.Move && !isAlreadyMove)
        {

            Vector2 tile = heroes[0].GetComponent<CharacterPosition>().tilePosition;
            Vector2 p = Global.ConvertTileToPosision((int)tile.x, (int)tile.y - 1);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                HandleMoveByDirection(CharacterDirection.eDirection.Down);
                isAlreadyMove=true;
            }
        }
        else if (state == eCharacterTurnState.Action && !isAlreadyAttack)
        {
            Vector2 tile = Global.ConvertPositionToTile(new Vector2(tileSelection.transform.position.x, tileSelection.transform.position.y));
            Vector2 p = Global.ConvertTileToPosision((int)tile.x, (int)tile.y - 1);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                tileSelection.transform.position = p;
                isAlreadyAttack = true;
            }
        }
    }
    public void MoveLeft()
    {
        if (heroes.Count > 1 && heroes[0].GetComponent<CharacterDirection>().direction == CharacterDirection.eDirection.Right) return;

        if (state == eCharacterTurnState.Move && !isAlreadyMove)
        {

            Vector2 tile = heroes[0].GetComponent<CharacterPosition>().tilePosition; 
            Vector2 p = Global.ConvertTileToPosision((int)tile.x - 1, (int)tile.y);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                HandleMoveByDirection(CharacterDirection.eDirection.Left);
                isAlreadyMove = true;
            }
        }
        else if (state == eCharacterTurnState.Action && !isAlreadyAttack)
        {
            Vector2 tile = Global.ConvertPositionToTile(new Vector2(tileSelection.transform.position.x, tileSelection.transform.position.y));
            Vector2 p = Global.ConvertTileToPosision((int)tile.x - 1, (int)tile.y);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                tileSelection.transform.position = p;
                isAlreadyAttack = true;
            }
        }
    }
    public void MoveRight()
    {
        if (heroes.Count > 1 && heroes[0].GetComponent<CharacterDirection>().direction == CharacterDirection.eDirection.Left) return;

        if (state == eCharacterTurnState.Move && !isAlreadyMove)
        {
            Vector2 tile = heroes[0].GetComponent<CharacterPosition>().tilePosition;
            Vector2 p = Global.ConvertTileToPosision((int)tile.x + 1, (int)tile.y);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                HandleMoveByDirection(CharacterDirection.eDirection.Right);
                isAlreadyMove = true;
            }
        }
        else if (state == eCharacterTurnState.Action && !isAlreadyAttack)
        {
            Vector2 tile = Global.ConvertPositionToTile(new Vector2(tileSelection.transform.position.x, tileSelection.transform.position.y));
            Vector2 p = Global.ConvertTileToPosision((int)tile.x + 1, (int)tile.y);
            if (GetComponent<PathHighlight>().ValidPath(p))
            {
                tileSelection.transform.position = p;
                isAlreadyAttack = true;
            }
        }
    }

    void HandleMoveByDirection(CharacterDirection.eDirection dir)
    {
        GameObject lastHero = null;
        for (int i = 0; i < heroes.Count; i++)
        {
            GameObject hero = heroes[i];
            if (i == 0)
            {
                //Position
                hero.GetComponent<CharacterPosition>().Backup();
                if(dir == CharacterDirection.eDirection.Up)
                    hero.GetComponent<CharacterPosition>().MoveUp();
                else if(dir == CharacterDirection.eDirection.Down)
                    hero.GetComponent<CharacterPosition>().MoveDown();
                else if(dir == CharacterDirection.eDirection.Left)
                    hero.GetComponent<CharacterPosition>().MoveLeft();
                else if(dir == CharacterDirection.eDirection.Right) 
                    hero.GetComponent<CharacterPosition>().MoveRight();
                //Direction
                hero.GetComponent<CharacterDirection>().Backup();
                hero.GetComponent<CharacterDirection>().direction = dir;
                hero.GetComponent<CharacterDirection>().HandleDirection();
            }
            else
            {
                //Position
                hero.GetComponent<CharacterPosition>().Backup();
                hero.GetComponent<CharacterPosition>().tilePosition = lastHero.GetComponent<CharacterPosition>().lastTilePosition;
                //Direction
                hero.GetComponent<CharacterDirection>().Backup();
                hero.GetComponent<CharacterDirection>().direction = lastHero.GetComponent<CharacterDirection>().lastDirection;
                hero.GetComponent<CharacterDirection>().HandleDirection();
            }
            lastHero = hero;
        }
    }
    void RestoreMove()
    {
        for (int i = 0; i < heroes.Count; i++)
        {
            GameObject hero = heroes[i];
            hero.GetComponent<CharacterPosition>().Restore();
            hero.GetComponent<CharacterDirection>().Restore();
        }
    }
    public void AddHero(GameObject hero)
    {
        int count = heroes.Count;
        if(count == 0)
        {
            heroes.Add(hero);
            GameManager.instance.collectableHeroes.Remove(hero);
            return;
        }
        else
        {
            var lastHero = heroes[count - 1];
            var dir = lastHero.GetComponent<CharacterDirection>().direction;
            var tilePosition = lastHero.GetComponent<CharacterPosition>().tilePosition;
            switch (dir)
            {
                case CharacterDirection.eDirection.Up:
                    {
                        hero.GetComponent<CharacterPosition>().tilePosition = new Vector2Int(tilePosition.x, tilePosition.y - 1);
                        hero.GetComponent<CharacterDirection>().direction = dir;
                        break;
                    }
                case CharacterDirection.eDirection.Down:
                    {
                        hero.GetComponent<CharacterPosition>().tilePosition = new Vector2Int(tilePosition.x, tilePosition.y + 1);
                        hero.GetComponent<CharacterDirection>().direction = dir;
                        break;
                    }
                case CharacterDirection.eDirection.Left:
                    {
                        hero.GetComponent<CharacterPosition>().tilePosition = new Vector2Int(tilePosition.x + 1, tilePosition.y);
                        hero.GetComponent<CharacterDirection>().direction = dir;
                        break;
                    }
                case CharacterDirection.eDirection.Right:
                    {
                        hero.GetComponent<CharacterPosition>().tilePosition = new Vector2Int(tilePosition.x - 1, tilePosition.y);
                        hero.GetComponent<CharacterDirection>().direction = dir;
                        break;
                    }
            }
            heroes.Add(hero);
            GameManager.instance.collectableHeroes.Remove(hero);
        }
    }
    void CheckCollideCollectableHero()
    {
        var allHero = GameObject.FindGameObjectsWithTag("Hero");
        var tilePosition = heroes[0].GetComponent<CharacterPosition>().tilePosition;
        foreach (var hero in allHero)
        {
            //filter
            bool check = false;
            foreach(var h in heroes)
            {
                if(hero == h)
                {
                    check = true;
                    break;
                }
            }
            if (check) continue;

            var chr = hero.GetComponent<CharacterPosition>();
            if (chr == null) continue;

            if (chr.tilePosition == tilePosition)
            {
                AddHero(hero);
                break;
            }
        }
    }
    public void RotateCharacterRight()
    {
        if (heroes.Count < 2) return;
        if (isAlreadyMove && state == eCharacterTurnState.Move) return;

        var first = heroes[0];
        var last = heroes[heroes.Count - 1];

        var newList = new List<GameObject>();
        var rotateBackup = new List<RotateCharacterData>();
        for (var i = 0; i < heroes.Count; i++)
        {
            var rot = new RotateCharacterData();
            rot.tilePosition = heroes[i].GetComponent<CharacterPosition>().tilePosition;
            rot.direction = heroes[i].GetComponent<CharacterDirection>().direction;
            rotateBackup.Add(rot);
        }

        //Reorder
        newList.Add(last);
        for (var i = 0; i < heroes.Count-1; i++)
        {
            newList.Add(heroes[i]);
        }

        //calculate position
        for (var i = 0; i < newList.Count; i++)
        {
            var h = newList[i];
            //h.GetComponent<CharacterPosition>().tilePosition = rotateBackup[i].tilePosition;
            h.GetComponent<CharacterPosition>().MoveTo(rotateBackup[i].tilePosition);
            h.GetComponent<CharacterDirection>().direction = rotateBackup[i].direction;
            h.GetComponent<CharacterDirection>().HandleDirection();
            h.GetComponent<CharacterDirection>().Backup();
            h.GetComponent<CharacterDirection>().StopBlink();
        }

        heroes.Clear();
        heroes = newList;
        if(state == eCharacterTurnState.Move)
        {
            heroes[0].GetComponent<CharacterDirection>().Blink();
        }
    }
    public void RotateCharacterLeft()
    {
        if (heroes.Count < 2) return;
        if (isAlreadyMove && state == eCharacterTurnState.Move) return;

        var first = heroes[0];

        var newList = new List<GameObject>();
        var rotateBackup = new List<RotateCharacterData>();
        for (var i = 0; i < heroes.Count; i++)
        {
            var rot = new RotateCharacterData();
            rot.tilePosition = heroes[i].GetComponent<CharacterPosition>().tilePosition;
            rot.direction = heroes[i].GetComponent<CharacterDirection>().direction;
            rotateBackup.Add(rot);
        }

        //Reorder
        for (var i = 1; i < heroes.Count; i++)
        {
            newList.Add(heroes[i]);
        }
        newList.Add(first);

        //calculate position
        for (var i = 0; i < newList.Count; i++)
        {
            var h = newList[i];
            //h.GetComponent<CharacterPosition>().tilePosition = rotateBackup[i].tilePosition;
            h.GetComponent<CharacterPosition>().MoveTo(rotateBackup[i].tilePosition);
            //h.GetComponent<CharacterPosition>().Backup();
            h.GetComponent<CharacterDirection>().direction = rotateBackup[i].direction;
            h.GetComponent<CharacterDirection>().HandleDirection();
            h.GetComponent<CharacterDirection>().Backup();
            h.GetComponent<CharacterDirection>().StopBlink();
        }

        heroes.Clear();
        heroes = newList;
        if (state == eCharacterTurnState.Move)
        {
            heroes[0].GetComponent<CharacterDirection>().Blink();
        }
    }
    public void RemoveHero()
    {
        if (heroes.Count == 1)
        {
            Destroy(heroes[0]);
            //Game Over
            GameManager.instance.GameOver();
            state = eCharacterTurnState.None;
            heroes[0].GetComponent<CharacterDirection>().StopBlink();
            PathHighlight path = GetComponent<PathHighlight>();
            path.Hide();
            isAlreadyMove = false;
            isAlreadyAttack = false;
            tileSelection.SetActive(false);

        }
        else
        {
            var first = heroes[0];

            var newList = new List<GameObject>();
            var rotateBackup = new List<RotateCharacterData>();
            for (var i = 0; i < heroes.Count; i++)
            {
                var rot = new RotateCharacterData();
                rot.tilePosition = heroes[i].GetComponent<CharacterPosition>().tilePosition;
                rot.direction = heroes[i].GetComponent<CharacterDirection>().direction;
                rotateBackup.Add(rot);
            }

            //Reorder
            for (var i = 1; i < heroes.Count; i++)
            {
                newList.Add(heroes[i]);
            }
            //newList.Add(first);

            //calculate position
            for (var i = 0; i < newList.Count; i++)
            {
                var h = newList[i];
                //h.GetComponent<CharacterPosition>().tilePosition = rotateBackup[i].tilePosition;
                h.GetComponent<CharacterPosition>().MoveTo(rotateBackup[i].tilePosition);
                //h.GetComponent<CharacterPosition>().Backup();
                h.GetComponent<CharacterDirection>().direction = rotateBackup[i].direction;
                h.GetComponent<CharacterDirection>().HandleDirection();
                h.GetComponent<CharacterDirection>().Backup();
                h.GetComponent<CharacterDirection>().StopBlink();
            }

            heroes.Clear();
            heroes = newList;

            Destroy(first);
        }
    }
    void CheckCollideItem()
    {
        foreach (var item in GameManager.instance.items)
        {
            if(item.transform.position == heroes[0].transform.position)
            {
                heroes[0].GetComponent<CharacterStatus>().GotItem(item);
                break;
            }
        }
    }
}
