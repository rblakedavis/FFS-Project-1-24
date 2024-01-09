
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Enemy", order = 1)]
public class Enemy : ScriptableObject
{

    public string enemyName;

    public float damage;
    public float health;
    public float curHealth;
    public Sprite sprite;

    public float expWorth;

    public float attacksPerSecond;
}
