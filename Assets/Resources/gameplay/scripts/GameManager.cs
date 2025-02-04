using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class MapTileData
{
    public float x;
    public float y;
    public float size;
    public GameObject tile;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    enum eGameState
    {
        None = 0,
        Init,
        PreGame,
        Play,
        PostGame,
        Result
    }
    eGameState state = eGameState.None;
    [Header("Perlin noise ")]
    [SerializeField] float perlinNoiseAdjust = 16;
    [SerializeField] float perlinNoiseHeight = 0.5f;

    [Header("Map Generate")]
    [SerializeField] GameObject mapContainer;
    [SerializeField] GameObject prefabBlock;
    [SerializeField] GameObject prefabBlock2;
    [SerializeField] int sizeX;
    [SerializeField] int sizeY;
    public MapTileData[,] map;

    [Header("Players")]
    [SerializeField] List<GameObject> prefabHeroes;
    [SerializeField] List<GameObject> prefabEnemies;
    public List<MapTileData> obstrucles = new List<MapTileData>();
    GameObject player;

    [Header("Hero Collectable")]
    public List<GameObject> collectableHeroes = new List<GameObject>();
    public List<GameObject> enemies = new List<GameObject>();
    bool isSpawningHero;
    bool isSpawningEnemy;

    [Header("Items")]
    [SerializeField] List<GameObject> prefabItems;
    public List<GameObject> items = new List<GameObject>();
    bool isSpawningItem;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        state = eGameState.None;
    }
    void GenerateMap()
    {
        map = new MapTileData[sizeX, sizeY];
        float x = 0;
        float y = 0;
        float ratio = 0.5f;
        for (int i = 0; i < sizeX; i++)
        {
            for(int j = 0; j < sizeY; j++)
            {
                x = i * ratio;
                y = j * ratio;
                var tile = Instantiate(prefabBlock, new Vector2(x, y),Quaternion.identity);
                tile.transform.SetParent(mapContainer.transform,false);

                var data = new MapTileData();
                data.x = x;
                data.y = y;
                data.size = 0.5f;
                data.tile = tile;
                map[i,j] = data;
            }
        }
        Camera.main.transform.position = new Vector3(x/2f,y/2f,-10);
    }
    void GernerateObstrucle()
    {
        obstrucles = new List<MapTileData>();
        float x = 0;
        float y = 0;
        float ratio = 0.5f;
        float px = Random.Range(0, 255);
        float py = Random.Range(0, 255);
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                float xCoord = px + (float)(i) / perlinNoiseAdjust;
                float yCoord = py + (float)(j) / perlinNoiseAdjust;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                Debug.Log(sample);
                if(sample > perlinNoiseHeight)
                {
                    x = i * ratio;
                    y = j * ratio;
                    var tile = Instantiate(prefabBlock2, new Vector2(x, y), Quaternion.identity);
                    tile.transform.SetParent(mapContainer.transform, false);

                    var data = new MapTileData();
                    data.x = x;
                    data.y = y;
                    data.size = 0.5f;
                    data.tile = tile;
                    obstrucles.Add(data);
                }
                
            }
        }
    }
    void CreatePlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<CharacterTurnbase>().AddHero(collectableHeroes[0]);
    }
    //Hero
    IEnumerator SpawnCollectableHero()
    {
        isSpawningHero = true;

        yield return new WaitForSeconds(0.5f);
        for(int i = 0;i < Global.MAX_HERO;i++)
        {
            CreateHero();
            yield return new WaitForSeconds(0.0f);
        }
        isSpawningHero=false;
    }
    void CreateHero()
    {
        var hero = GetRandomHero();
        hero.GetComponent<CharacterPosition>().tilePosition = GetRandomAvailableTile();
        hero.GetComponent<CharacterDirection>().direction = CharacterDirection.eDirection.Down;
        collectableHeroes.Add(hero);
    }
    GameObject GetRandomHero()
    {
        int r = Random.Range(0, prefabHeroes.Count);
        var hero = Instantiate(prefabHeroes[r], Vector3.zero, Quaternion.identity);
        return hero;
    }
    //Enemy
    IEnumerator SpawnEnemy()
    {
        isSpawningEnemy = true;

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < Global.MAX_ENEMY; i++)
        {
            CreateEnemy();
            yield return new WaitForSeconds(0.0f);
        }
        isSpawningEnemy = false;
    }

    IEnumerator SpawnItem()
    {
        isSpawningItem = true;

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < Global.MAX_ITEM; i++)
        {
            var item = GetRandomItem();
            var p = GetRandomAvailableTile();
            item.transform.position = Global.ConvertTileToPosision(p.x, p.y);
            items.Add(item);
            yield return new WaitForSeconds(0.0f);
        }
        isSpawningItem = false;
    }
    GameObject GetRandomItem()
    {
        int r = Random.Range(0, prefabItems.Count);
        var hero = Instantiate(prefabItems[r], Vector3.zero, Quaternion.identity);
        return hero;
    }
    Vector2Int GetRandomAvailableTile()
    {
        List<Vector2Int> tiles = new List<Vector2Int>();
        var heroes = GameObject.FindGameObjectsWithTag("Hero");
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Vector2Int t = new Vector2Int(i,j);
                Vector2 p = Global.ConvertTileToPosision(t.x,t.y);
                bool check = false;
               
                //Check objectrucle
                foreach (var obs in obstrucles)
                {
                    if(p.x == obs.x && p.y == obs.y)
                    {
                        check = true; 
                        break;
                    }
                }
                if (check) continue;

                //Check Heroes
                foreach (var hero in heroes)
                {
                    if (hero.GetComponent<CharacterPosition>().tilePosition == t)
                    {
                        check = true; 
                        break;
                    }
                }
                if (check) continue;

                //Check Enemy
                foreach (var enemy in enemies)
                {
                    if (enemy.GetComponent<CharacterPosition>().tilePosition == t)
                    {
                        check = true; 
                        break;
                    }
                }
                if (check) continue;

                //Check Item
                foreach (var item in items)
                {
                    Vector2 p2 = item.transform.position;
                    if (p2 == p)
                    {
                        check = true; 
                        break;
                    }
                }
                if (check) continue;

                tiles.Add(t);
            }
        }

        if (tiles.Count == 0) return new Vector2Int(0, 0);

        int r = Random.Range(0, tiles.Count);
        return tiles[r];
    }
    void CreateEnemy()
    {
        var hero = GetRandomEnemy();
        //int x = Random.Range(0, 16);
        //int y = Random.Range(0, 16);
        hero.GetComponent<CharacterPosition>().tilePosition = GetRandomAvailableTile();//new Vector2Int(x, y);
        enemies.Add(hero);
    }
    GameObject GetRandomEnemy()
    {
        int r = Random.Range(0, prefabEnemies.Count);
        var hero = Instantiate(prefabEnemies[r], Vector3.zero, Quaternion.identity);
        return hero;
    }
    public void GameOver()
    {
        state = eGameState.Result;
        UITurnbase.instance.Hide();
        UIResult.instance.Show();
    }
    void Update()
    {

        switch (state)
        {
            case eGameState.None: {
                    state = eGameState.Init;
                    break; 
                }
            case eGameState.Init: {
                    GenerateMap();
                    GernerateObstrucle();
                    StartCoroutine(SpawnCollectableHero());
                    StartCoroutine(SpawnEnemy());
                    StartCoroutine (SpawnItem());
                    UITurnNotify.instance.Show("START", () => {
                        state = eGameState.Play;
                        CreatePlayer();
                        TurnbaseManager.instance.StartTurn(TurnbaseManager.eTurnState.Player);
                    });
                    state = eGameState.PreGame;
                    break; 
                }
            case eGameState.PreGame: { 
                    break; 
                }
            case eGameState.Play: {
                    HandleSpawnItem();
                    HandleSpawnEnemy();
                    HandleSpawnHero();
                    break; 
                }
            case eGameState.PostGame: { break; }
            case eGameState.Result: { break; }
        }
    }
    void HandleSpawnHero()
    {
        if (isSpawningHero) return;

        if (collectableHeroes.Count == 0)
        {
            StartCoroutine(SpawnCollectableHero());
        }
    }
    void HandleSpawnEnemy()
    {
        if (isSpawningEnemy) return;

        if (enemies.Count == 0)
        {
            StartCoroutine(SpawnEnemy());
        }
    }
    void HandleSpawnItem()
    {
        if (isSpawningItem) return;

        if(items.Count == 0)
        {
            StartCoroutine(SpawnItem());
        }
    }
}
