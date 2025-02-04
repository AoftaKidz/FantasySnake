using UnityEngine;

public class Global
{
    public static int MAX_HERO = 2;
    public static int MAX_ENEMY = 2;
    public static int MAX_ITEM = 3;
    public static int POWERUP_CHANCE_STEP = 5;

    public static Vector2Int WARRIOR_HP_RANGE = new Vector2Int(10,15);
    public static Vector2Int WARRIOR_ATTACK_RANGE = new Vector2Int(2,5);
    public static Vector2Int WARRIOR_DEF_RANGE = new Vector2Int(2,5);
    public static Vector2Int ROGUE_HP_RANGE = new Vector2Int(7, 12);
    public static Vector2Int ROGUE_ATTACK_RANGE = new Vector2Int(3, 6);
    public static Vector2Int ROGUE_DEF_RANGE = new Vector2Int(1, 3);
    public static Vector2Int WIZARD_HP_RANGE = new Vector2Int(5, 10);
    public static Vector2Int WIZARD_ATTACK_RANGE = new Vector2Int(2, 5);
    public static Vector2Int WIZARD_DEF_RANGE = new Vector2Int(1, 3);
    public static Vector2 ConvertTileToPosision(int x,int y)
    {
        return new Vector2(x * 0.5f,y*0.5f);
    }
    public static Vector2 ConvertPositionToTile(Vector2 pos)
    {
        int x = (int)( pos.x / 0.5f);
        int y = (int)(pos.y / 0.5f);
        return new Vector2(x, y);
    }
}
