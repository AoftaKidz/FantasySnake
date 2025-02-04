using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnbaseManager : MonoBehaviour
{
    public static TurnbaseManager instance;
    public enum eTurnState
    {
        Player = 0,
        Enemy,
        None
    }
    public eTurnState state = eTurnState.None;
    public GameObject[] enemies;
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        switch (state)
        {
            case eTurnState.None:
                {
                    break;
                }
            case eTurnState.Player:
                {
                    break;
                }
            case eTurnState.Enemy:
                {
                    break;
                }
        }
    }
    public void StartTurn(eTurnState turn)
    {
        if(turn == eTurnState.None)
        {

        }else if(turn == eTurnState.Player)
        {
            UITurnNotify.instance.Show("PLAYER TURN", () => {
                UITurnbase.instance.Show();

                //Update powerup chance
                StartCoroutine(DoUpdateHeroPowerupChance());
            });
        }else if (turn == eTurnState.Enemy)
        {
            UITurnNotify.instance.Show("ENEMY TURN", () => {
                enemies = GameObject.FindGameObjectsWithTag("Enemy");
                StartCoroutine(DoSimpleEnemyTurn());
            });
        }
    }
    IEnumerator DoUpdateHeroPowerupChance()
    {
        var heroes = GameObject.FindGameObjectsWithTag("Hero");
        foreach (GameObject hero in heroes)
        {
            hero.GetComponent<CharacterStatus>().UpdatePowerupChance();
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator DoSimpleEnemyTurn()
    {
        foreach (GameObject enemy in enemies) {

            enemy.GetComponent<CharacterDirection>().Blink();
            enemy.GetComponent<CharacterStatus>().UpdatePowerupChance();
            yield return new WaitForSeconds(1);
            BasicMove(enemy);
        }

        yield return new WaitForSeconds(1);
        StartTurn(eTurnState.Player);
    }
    void BasicMove(GameObject enemy)
    {
        List<Vector2Int> paths = new List<Vector2Int>();
        var heroes = GameObject.FindGameObjectsWithTag("Hero");
        List<GameObject> allCharacters = new List<GameObject>();
        //all heroes
        foreach (var hero in heroes)
        {
            allCharacters.Add(hero);
        }
        //enemies
        foreach (var e in enemies)
        {
            allCharacters.Add(e);
        }
        Vector2Int tilePosition = enemy.GetComponent<CharacterPosition>().tilePosition;

        //Left
        if (CheckTile(allCharacters.ToArray(), new Vector2Int(tilePosition.x - 1, tilePosition.y)))
        {
            paths.Add(new Vector2Int(tilePosition.x - 1, tilePosition.y));
        }

        //Right
        if (CheckTile(allCharacters.ToArray(), new Vector2Int(tilePosition.x + 1, tilePosition.y)))
        {
            paths.Add(new Vector2Int(tilePosition.x + 1, tilePosition.y));
        }

        //Up
        if (CheckTile(allCharacters.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y + 1)))
        {
            paths.Add(new Vector2Int(tilePosition.x , tilePosition.y + 1));
        }

        //Down
        if (CheckTile(allCharacters.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y - 1)))
        {
            paths.Add(new Vector2Int(tilePosition.x , tilePosition.y - 1));
        }

        //Random path
        int r = Random.Range(0, paths.Count);
        enemy.GetComponent<CharacterPosition>().MoveTo(paths[r]);
        enemy.GetComponent<CharacterDirection>().StopBlink();
    }
    bool CheckTile(GameObject[] allheroes, Vector2Int tile)
    {
        //check border
        if (tile.x < 0 || tile.x >= 16 || tile.y < 0 || tile.y >= 16) return false;
        for (int i = 0; i < allheroes.Length; i++)
        {
            if (allheroes[i].GetComponent<CharacterPosition>().tilePosition == tile) return false;
        }

        //Check Obstrucle
        for (int i = 0; i < GameManager.instance.obstrucles.Count; i++)
        {
            Vector2 t = Global.ConvertPositionToTile(new Vector2(GameManager.instance.obstrucles[i].x, GameManager.instance.obstrucles[i].y));
            Vector2Int t1 = new Vector2Int((int)t.x, (int)t.y);
            if (t1 == tile) return false;
        }

        return true;
    }
}
