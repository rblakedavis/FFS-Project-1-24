using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp;
    public float maxHP;
    public float hpRegen;

    public int level;
    public int experience;
    public int expNextLevel = 1; //placeholder for testing


    public float magic;
    public float maxMagic;
    public float magicRegen;

    public float attack;
    public float defense;
    public Dictionary<string, float> levelUp;




    private static Player _instance;
    private bool isInitialized = false;

    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Player>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(Player).Name);
                    _instance = singleton.AddComponent<Player>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (GameManager.Instance != null)
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            StartCoroutine(WaitForGameManager());
        }
    }

    private void Start()
    {
        levelUp = new Dictionary<string, float>
        {
            {"maxHP", 2f + .2f*Player.Instance.level},
            {"hpRegen", .05f +.02f*Player.Instance.level},
            {"maxMagic", .5f + .1f *Player.Instance.level },
            {"magicRegen", .03f + .01f*Player.Instance.level},
            {"attack", .5f + .01f*Player.Instance.level},
            {"defense", .4f + .01f*Player.Instance.level },
        };
    }

    private IEnumerator WaitForGameManager()
    {
        yield return null;
        Awake(); // Call Awake after waiting for a frame
    }
}
