using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "GameData", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public string curZoneName;
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
    
    //Player stats    
    public int levelCur = 1;
    public int magicMax = 2;
    public int magicCur = 0;
    public int healthMax = 15;
    public int healthCur = 15;
    public int goldCur;

    public string subWindowText = "example text";    

    //Game object names    
    public string enemyImageName = "BaddiesWindow";

}
