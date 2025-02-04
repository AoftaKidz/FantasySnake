using UnityEngine;

public class CharacterPosition : MonoBehaviour
{
    public Vector2Int tilePosition = Vector2Int.zero;
    public Vector2Int lastTilePosition = Vector2Int.zero;
    public Vector2Int moveTilePosition = Vector2Int.zero;
    bool isMoving = false;
    CharacterAttack attack;

    private void Start()
    {
        lastTilePosition = tilePosition;
        attack = GetComponent<CharacterAttack>();
    }
    void Update()
    {
        if (attack.isAttacking) return;

        if (isMoving)
        {
            var p1 = Global.ConvertTileToPosision(moveTilePosition.x, moveTilePosition.y);
            var p2 = new Vector2(transform.position.x, transform.position.y);
            var d = p1 - p2;
            if(d.magnitude < 0.1f)
            {
                //Reach target
                isMoving = false;
                tilePosition = moveTilePosition;
                lastTilePosition = tilePosition;
            }
            else
            {
                Vector3 p = new Vector3(d.x, d.y, 0);
                transform.position += p * 20f * Time.deltaTime;
            }
        }
        else
        {
            transform.position = Global.ConvertTileToPosision(tilePosition.x, tilePosition.y);
        }
    }
    public void MoveTo(Vector2Int tile)
    {
        moveTilePosition = tile;
        isMoving = true;
    }
    public void Backup()
    {
        lastTilePosition = tilePosition;
    }
    public void Restore()
    {
        tilePosition = lastTilePosition;
    }
    public void MoveUp()
    {
        lastTilePosition = tilePosition;
        tilePosition.y++;
    }
    public void MoveDown()
    {
        lastTilePosition = tilePosition;
        tilePosition.y--;
    }
    public void MoveLeft()
    {
        lastTilePosition = tilePosition;
        tilePosition.x--;
    }
    public void MoveRight()
    {
        lastTilePosition = tilePosition;
        tilePosition.x++;
    }
}
