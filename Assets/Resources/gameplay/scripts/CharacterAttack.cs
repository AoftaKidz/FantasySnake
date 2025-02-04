using UnityEngine;
using System;
using System.Collections;

public struct DamageData
{
    public bool canCounter;
    public float damage;
    public CharacterStatus.eHeroClass heroClass;
    public GameObject hero;
}
public class CharacterAttack : MonoBehaviour
{
    CharacterStatus status;
    public bool isAttacking = false;
    GameObject target;
    Action callback;
    bool canCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        status = GetComponent<CharacterStatus>();
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            var p1 = target.transform.position;//Global.ConvertTileToPosision(moveTilePosition.x, moveTilePosition.y);
            var p2 = transform.position;//new Vector2(transform.position.x, transform.position.y);
            var d = p1 - p2;
            if (d.magnitude < 0.1f)
            {
                //Reach target
                isAttacking = false;
                var damage = new DamageData();
                damage.damage = status.attack;
                damage.heroClass = status.heroClass;
                damage.hero = gameObject;
                damage.canCounter = canCounter;
                target.GetComponent<CharacterAttack>().GetHit(damage);

                //go back to position
                GetComponent<CharacterPosition>().Restore();
                if(callback != null)
                {
                    callback();
                }
            }
            else
            {
                Vector3 p = new Vector3(d.x, d.y, 0);
                transform.position += p * 20f * Time.deltaTime;
            }
        }
    }
    public void Attacking(GameObject target,Action callback,bool canCounter = true)
    {
        if (isAttacking) return;

        isAttacking = true;
        this.target = target;
        this.callback = callback;
        this.canCounter = canCounter;
    }
    public void GetHit(DamageData damage)
    {
        float damageAmp = 1;
        if(status.heroClass == CharacterStatus.eHeroClass.Warrior && damage.heroClass == CharacterStatus.eHeroClass.Wizard)
        {
            damageAmp = 2;
        }else if (status.heroClass == CharacterStatus.eHeroClass.Rogue && damage.heroClass == CharacterStatus.eHeroClass.Warrior)
        {
            damageAmp = 2;
        }
        else if (status.heroClass == CharacterStatus.eHeroClass.Wizard && damage.heroClass == CharacterStatus.eHeroClass.Rogue)
        {
            damageAmp = 2;
        }
        float dmg = damage.damage - status.defense;
        if(dmg <= 0)
        {
            dmg = 1;
        }

        //Show text damage
        var prefab = Resources.Load("gameplay\\prefabs\\FloatingText") as GameObject;
        Vector3 p = transform.position + Vector3.up * 0.5f;
        var text = Instantiate(prefab, p, Quaternion.identity);
        if(damageAmp == 2)
            text.GetComponent<FloatingText>().SetText($"-{damageAmp * dmg}",Color.yellow);
        else
            text.GetComponent<FloatingText>().SetText($"-{damageAmp * dmg}", Color.red);

        //Decrease hp
        status.health -= damageAmp * dmg;
        if(status.health <= 0)
        {
            if (CharacterTurnbase.instance.heroes[0] == gameObject)
            {
                //Remove Hero
                CharacterTurnbase.instance.RemoveHero();
            }
            else
            {
                //Remove Enemy
                GameManager.instance.enemies.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        else
        {
            //Counter
            if(damage.canCounter)
                StartCoroutine(CounterAttack(damage.hero));
        }
    }
    IEnumerator CounterAttack(GameObject enemy)
    {
        yield return new WaitForSeconds(1);
        Attacking(enemy, null, false);
    }
}
