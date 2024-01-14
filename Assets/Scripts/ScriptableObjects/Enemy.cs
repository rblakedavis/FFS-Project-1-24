
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public int index;
    public int bossIndex;

    public Sprite sprite;
    public string enemyName;

    public float damage;
    public float damageModifier;

    public float maxHealth;
    public float curHealth;


    public int expWorth;

    public float secondsBetweenAttacks;

}
