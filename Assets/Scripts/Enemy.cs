using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float damage;
    public float HP;
    static bool playerTurn;
    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTurn == false) {
            Attack(player, damage);
        }
    }
    void Attack(Player player, float damage){
        player.HP -= damage;
    }
}
