using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms;

public class PathHighlight : MonoBehaviour
{
    public List<GameObject> paths = new List<GameObject>();
    [SerializeField] GameObject prefabPath;
    [SerializeField] GameObject prefabPathRed;

    public void Show(CharacterDirection.eDirection dir, Vector2Int tilePosition,bool isMove = true)
    {
        foreach (GameObject path in paths)
        {
            Destroy(path);
        }
        paths.Clear();

        if (isMove)
        {
            CreateMovePaths(dir, tilePosition);
        }
        else
        {
            CreateAttackPaths(dir, tilePosition);
        }
    }
    void CreateMovePaths(CharacterDirection.eDirection dir, Vector2Int tilePosition)
    {
        List<GameObject> allheroes = new List<GameObject>();
        foreach (var h in GetComponent<CharacterTurnbase>().heroes)
        {
            allheroes.Add(h);
        }
        
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //allheroes.AddRange(enemies);
        foreach (var e in enemies)
        {
            allheroes.Add(e);
        }
        
        //Left
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x - 1, tilePosition.y)))
        {
            var path = Instantiate(prefabPath, Global.ConvertTileToPosision(tilePosition.x - 1, tilePosition.y), Quaternion.identity);
            paths.Add(path);
        }

        //Right
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x + 1, tilePosition.y)))
        {
            var path = Instantiate(prefabPath, Global.ConvertTileToPosision(tilePosition.x + 1, tilePosition.y), Quaternion.identity);
            paths.Add(path);
        }

        //Up
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y + 1)))
        {
            var path = Instantiate(prefabPath, Global.ConvertTileToPosision(tilePosition.x, tilePosition.y + 1), Quaternion.identity);
            paths.Add(path);
        }

        //Down
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y - 1)))
        {
            var path = Instantiate(prefabPath, Global.ConvertTileToPosision(tilePosition.x, tilePosition.y - 1), Quaternion.identity);
            paths.Add(path);
        }
    }
    void CreateAttackPaths(CharacterDirection.eDirection dir, Vector2Int tilePosition)
    {
        List<GameObject> allheroes = new List<GameObject>();
        foreach (var h in GetComponent<CharacterTurnbase>().heroes)
        {
            allheroes.Add(h);
        }

        //Left
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x - 1, tilePosition.y)))
        {
            var path = Instantiate(prefabPathRed, Global.ConvertTileToPosision(tilePosition.x - 1, tilePosition.y), Quaternion.identity);
            paths.Add(path);
        }

        //Right
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x + 1, tilePosition.y)))
        {
            var path = Instantiate(prefabPathRed, Global.ConvertTileToPosision(tilePosition.x + 1, tilePosition.y), Quaternion.identity);
            paths.Add(path);
        }

        //Up
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y + 1)))
        {
            var path = Instantiate(prefabPathRed, Global.ConvertTileToPosision(tilePosition.x, tilePosition.y + 1), Quaternion.identity);
            paths.Add(path);
        }

        //Down
        if (CheckTile(allheroes.ToArray(), new Vector2Int(tilePosition.x, tilePosition.y - 1)))
        {
            var path = Instantiate(prefabPathRed, Global.ConvertTileToPosision(tilePosition.x, tilePosition.y - 1), Quaternion.identity);
            paths.Add(path);
        }
    }
    bool CheckTile(GameObject[]allheroes,Vector2Int tile)
    {
        //check border
        if(tile.x < 0 || tile.x >= 16 || tile.y < 0 || tile.y >= 16) return false;

        //Check Obstrucle
        for (int i = 0; i < GameManager.instance.obstrucles.Count; i++)
        {
            Vector2 t = Global.ConvertPositionToTile(new Vector2(GameManager.instance.obstrucles[i].x, GameManager.instance.obstrucles[i].y));
            Vector2Int t1 = new Vector2Int((int)t.x,(int)t.y);
            if (t1 == tile) return false;
        }

        //Check hero
        for (int i = 0; i < allheroes.Length; i++)
        {
            if (allheroes[i].GetComponent<CharacterPosition>().tilePosition ==  tile) return false;
        }

        
        return true;
    }
    public void Hide()
    {
        foreach (GameObject path in paths)
        {
            Destroy(path);
        }
        paths.Clear();
    }
    public bool ValidPath(Vector3 pos)
    {
        foreach(GameObject path in paths)
        {
            if(path.transform.position == pos) return true;
        }
        return false;
    }
}
