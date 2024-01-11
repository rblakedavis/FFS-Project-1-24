
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{
    public int index;

    public Sprite sprite;
    public string enemyName;

    public float damage;
    public float damageModifier;

    public float health;
    public float curHealth;


    public float expWorth;

    public float attacksPerSecond;

}
