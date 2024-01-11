using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float hp;
    public float maxHP;
    public int level;
    public int magic;
    public int maxMagic;
    public float attack;
    public float healAmount = 0.25f;

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

                // Initialize player stats using GameManager data
                if (GameManager.Instance.gameData != null)
                {
                    hp = GameManager.Instance.gameData.hp;
                    maxHP = GameManager.Instance.gameData.maxHP;
                    level = GameManager.Instance.gameData.level;
                    magic = GameManager.Instance.gameData.magic;
                    maxMagic = GameManager.Instance.gameData.maxMagic;

                    attack = GameManager.Instance.gameData.attack;
                }
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

    private IEnumerator WaitForGameManager()
    {
        yield return null;
        Awake(); // Call Awake after waiting for a frame
    }
}
