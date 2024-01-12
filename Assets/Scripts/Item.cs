using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string typeOfItem; //Is it a weapon, armor, consumeable? 
    public float addsToPlayerAttack;
    public float addsToPlayerDefense;
    public float addsToPlayerMaxMagic;
    public float restoresPlayerMagic;
    public float addsToPlayerMagicRegen;
    public float addsToPlayerMaxHealth;
    public float restoresPlayerHealth;
    public float addsToPlayerHealthRegen;
}
