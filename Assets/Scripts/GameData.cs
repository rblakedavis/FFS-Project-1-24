using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{

    //public string curZoneName;
    public int curZoneIndex = 0;
    public string[] zoneNames = {"Forest", "Cave", "Ruins", "Depths", "Underworld"};
    
    //Placeholder for the number of available enemies
    //in the current zone.
    public int zoneNumEnemies = 1; 
   
    //Zone offset is equal to the cumulative number of
    //enemies that are available in all previous zones.
    public int[] zoneOffset = { 0, 3, 6, 9, 12 };

    public Sprite[] enemySprites;

    public Enemy[] enemyList;
    
    public int curEnemy;

    public string[] enemyNames = 
    {
        "wolf",     "forest2",  "forest3", 
        "cave1",    "cave2",    "cave3",
        "ruins1",   "ruins2",  "ruins3",
        "depths1",   "depths2",     "depths3",
        "underworld1",  "underworld2",  "underworld3"
    };
    public float[] enemyAttackValues =
    {
        2f,     3f,     1f,
        3f,     5f,     5f,
        8f,     10f,    11f,
        14f,    16f,    17f,
        20f,    22f,    24f
    };
    public float[] enemyMaxHealthValues =
    {
        15f,    12f,    40f,
        20f,    20f,    25f,
        35f,    35f,    40f,
        50f,    56f,    57f,
        80f,    99f,    95f
    };
    public int[] enemyExperienceValues =
    {
        2,     3,     1,
        3,     5,     5,
        8,     10,    11,
        14,    16,    17,
        20,    22,    24
    };

    //Player stats    
    public int level= 1;

    public float maxMagic = 2;
    public float magic = 0;
    public float magicRegen = 0.2f;


    public float maxHP = 15;
    public float hp = 15;
    public float hpRegen = 0.3f;

    public int goldCur;
    public float attack = 1;
    public float defense = 0.2f;

    public string subWindowText = "example text";    

    //Game object names    
    public string enemyImageName = "BaddiesWindow";

    public string[] winQuotes =
    {
        "Enemy defeated. You lick off the blood on your weapon.",
        "Victory achieved! The scent of triumph hangs in the air.",
        "Foe vanquished. You wipe the sweat from your brow.",
        "Enemy crushed. Your weapon gleams with the essence of conquest.",
        "The adversary falls. Adrenaline courses through your veins.",
        "Enemy Defeated! The echoes of battle fade.",
        "Nuisance eliminated.A testament to your prowess.",
        "Target neutralized. The taste of victory lingers the air.",
        "Opponent overcome. Victory achieved.",
        "Enemy eradicated. The shadows of conflict dissipate.",
        "Adversary slain. Your weapon thirsts for more."


    };

}
