using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    public float damage;
    

    public void Attack(Enemy Target) {
        Target.HP -= damage;
    }
}
