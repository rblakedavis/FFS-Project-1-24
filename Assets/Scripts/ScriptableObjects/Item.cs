using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu (fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Item : ScriptableObject
{
    public string itemName;

    public string typeOfItem; //Is it a weapon, armor, consumeable? 
    public float addsToPlayerAttack;
    public float addsToPlayerDefense;
    public float addsToPlayerMaxMagic;
    public float restoresPlayerMagic;
    public float addsToPlayerMagicRegen;
    public float addsToPlayerMaxHealth;
    public float restoresPlayerHealth;
    public float addsToPlayerHealthRegen;
    public int cost;
    
    [TextArea(3, 5)] public string description;

    public int minLevelAvailable;
    public int minZoneAvailable;

    public void itemPurchased(GameObject button)
    {
        GameData.Instance.goldCur -= cost;
        
        if (typeOfItem != "Consumable")
        {
            Player.Instance.attack += addsToPlayerAttack;
            Player.Instance.defense += addsToPlayerDefense;
            Player.Instance.maxMagic += addsToPlayerMaxMagic;
            Player.Instance.magicRegen += addsToPlayerMagicRegen;
            Player.Instance.maxHP += addsToPlayerMaxHealth;
            Player.Instance.hpRegen += addsToPlayerHealthRegen;
            Destroy(button);
        }
        else
        {
            Player.Instance.hp += restoresPlayerHealth;
            Player.Instance.magic += restoresPlayerMagic;
        }



    }
}
