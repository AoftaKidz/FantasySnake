using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public enum eHeroClass
    {
        Warrior = 0,
        Rogue,
        Wizard
    }
    [SerializeField] Vector2Int hpRange = Vector2Int.one;
    [SerializeField] Vector2Int defRange = Vector2Int.one;
    [SerializeField] Vector2Int atkRange = Vector2Int.one;
    public eHeroClass heroClass;
    public float maxHealth;
    public float health;
    public float defense;
    public float attack;
    float powerupChance = 0;
    private void Start()
    {
        if(heroClass == eHeroClass.Warrior)
        {
            maxHealth = Random.Range(Global.WARRIOR_HP_RANGE.x, Global.WARRIOR_HP_RANGE.y);
            defense = Random.Range(Global.WARRIOR_DEF_RANGE.x, Global.WARRIOR_DEF_RANGE.y);
            attack = Random.Range(Global.WARRIOR_ATTACK_RANGE.x, Global.WARRIOR_ATTACK_RANGE.y);
        }
        else if(heroClass==eHeroClass.Rogue)
        {
            maxHealth = Random.Range(Global.ROGUE_HP_RANGE.x, Global.ROGUE_HP_RANGE.y);
            defense = Random.Range(Global.ROGUE_DEF_RANGE.x, Global.ROGUE_DEF_RANGE.y);
            attack = Random.Range(Global.ROGUE_ATTACK_RANGE.x, Global.ROGUE_ATTACK_RANGE.y);
        }
        else
        {
            maxHealth = Random.Range(Global.WIZARD_HP_RANGE.x, Global.WIZARD_HP_RANGE.y);
            defense = Random.Range(Global.WIZARD_DEF_RANGE.x, Global.WIZARD_DEF_RANGE.y);
            attack = Random.Range(Global.WIZARD_ATTACK_RANGE.x, Global.WIZARD_ATTACK_RANGE.y);
        }
        health = maxHealth;
    }
    public void Attack(GameObject target)
    {
        if(target == null) return;
        if(target.TryGetComponent(out CharacterStatus status))
        {
            float damage = attack - status.defense;
            if(damage > 0)
            {
                status.health -= damage;
                if(status.health <= 0)
                {
                    Destroy(target);
                }
            }
        }
    }
    public void UpdatePowerupChance()
    {
        powerupChance += Global.POWERUP_CHANCE_STEP;
        int r = Random.Range(0, 100);
        if(r <= powerupChance)
        {
            powerupChance = 0;
            //Random status
            r = Random.Range(0, 3);
            if(r == 0)
            {
                //HP
                health++;
                maxHealth++;
                //Show text damage
                var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
                Vector3 p = transform.position + Vector3.up * 0.5f;
                var text = Instantiate(prefab, p, Quaternion.identity);
                    text.GetComponent<FloatingText>().SetText($"+{1}HP", Color.green);
            }else if (r == 1)
            {
                //Def
                defense++;
                //Show text damage
                var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
                Vector3 p = transform.position + Vector3.up * 0.5f;
                var text = Instantiate(prefab, p, Quaternion.identity);
                text.GetComponent<FloatingText>().SetText($"+{1}DEF", Color.green);
            }
            else
            {
                //Def
                attack++;
                //Show text damage
                var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
                Vector3 p = transform.position + Vector3.up * 0.5f;
                var text = Instantiate(prefab, p, Quaternion.identity);
                text.GetComponent<FloatingText>().SetText($"+{1}ATK", Color.green);
            }
        }
    }
    public void GotItem(GameObject item)
    {
        if (item == null) return;

        Item i = item.GetComponent<Item>();
        if(i.health > 0)
        {
            //HP
            health += i.health;
            if(health > maxHealth)
                health = maxHealth;
            //Show text damage
            var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
            Vector3 p = transform.position + Vector3.up * 0.5f;
            var text = Instantiate(prefab, p, Quaternion.identity);
            text.GetComponent<FloatingText>().SetText($"+{i.health}HP", Color.green);
        }else if (i.defense > 0)
        {
            //DEF
            defense += i.defense;
            //Show text damage
            var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
            Vector3 p = transform.position + Vector3.up * 0.5f;
            var text = Instantiate(prefab, p, Quaternion.identity);
            text.GetComponent<FloatingText>().SetText($"+{i.defense}DEF", Color.green);
        }
        else if (i.attack > 0)
        {
            //DEF
            attack += i.attack;
            //Show text damage
            var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
            Vector3 p = transform.position + Vector3.up * 0.5f;
            var text = Instantiate(prefab, p, Quaternion.identity);
            text.GetComponent<FloatingText>().SetText($"+{i.attack}ATK", Color.green);
        }

        GameManager.instance.items.Remove(item);
        Destroy(item);
    }
}
