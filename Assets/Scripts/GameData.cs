using System.Collections.Generic;
using System;
using UnityEngine;






public class GameData : MonoBehaviour
{


    //public string curZoneName;
    public int curZoneIndex = 0;
    public string[] zoneNames = { "Forest", "Cave", "Ruins", "Depths", "Underworld" };

    //Placeholder for the number of available enemies
    //in the current zone.
    public int zoneNumEnemies = 1;

    //Zone offset is equal to the cumulative number of
    //enemies that are available in all previous zones.
    public int[] zoneOffset = { 0, 3, 6, 9, 12 };

    public Sprite[] enemySprites;

    public Enemy[] enemyList;
    public Enemy[] bossList;



    public int curEnemy;


    public int goldCur;

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


    private static GameData _instance;
    private bool isInitialized = false;

    public static GameData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameData>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameData).Name);
                    _instance = singleton.AddComponent<GameData>();
                }
            }
            return _instance;
        }
    }



    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (GameManager.Instance != null)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);

                // Initialize game manager
                //blah blah blah...
                isInitialized = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}


