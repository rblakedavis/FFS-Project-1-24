using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zone;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI magic;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI subWindow;

    [SerializeField] private Image healthBar;
    [SerializeField] private Image magicBar;

    private static GameData resetGameData;

    public UnityEvent onLevelUp;


    private static GameManager _instance;
    private bool isInitialized = false;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).Name);
                    _instance = singleton.AddComponent<GameManager>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    public GameData gameData;

    private void Awake()
    {


        StartCoroutine(WaitForLoad());

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_instance);
        Debug.Log("Awake");

        if (_instance != this)
        {
            if (_instance.isInitialized)
            {
                DestroyImmediate(_instance.transform.gameObject);
                _instance = this;
                isInitialized = true;
                if (subWindow != null)
                {
                    if (SceneManager.GetActiveScene().name == "Main" && subWindow.text != gameData.subWindowText)
                    {
                        subWindow.text = gameData.subWindowText;
                        zone.text = gameData.zoneNames[gameData.curZoneIndex];
                        level.text = Player.Instance.level.ToString();
                        magic.text = Player.Instance.magic.ToString();
                        health.text = Player.Instance.hp.ToString();
                        healthBar.fillAmount = Player.Instance.hp / Player.Instance.maxHP;
                        magicBar.fillAmount = Player.Instance.magic / Player.Instance.maxMagic;
                    }
                }
            }
            else { DestroyImmediate(this); Debug.Log("Destroying this..."); }


        }


        if (resetGameData == null) 
        {
            resetGameData = ScriptableObject.CreateInstance<GameData>();
            resetGameData.enemyList = gameData.enemyList;
            resetGameData.enemySprites = gameData.enemySprites;

            DontDestroyOnLoad(resetGameData);
        }



        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    

    private void OnDestroy()
    {
        Debug.Log("Destroyed");
    }

    private void Start()
    {
        if (!isInitialized && SceneManager.GetActiveScene().name == "Main")
        {
            Debug.Log("Start");

            gameData.subWindowText = "Welcome to the game.";
            subWindow.text = gameData.subWindowText;
            zone.text = gameData.zoneNames[gameData.curZoneIndex];
            level.text = Player.Instance.level.ToString();
            magic.text = Player.Instance.magic.ToString();
            health.text = Player.Instance.hp.ToString();

            Player.Instance.hp = gameData.hp;
            Player.Instance.maxHP = gameData.maxHP;
            Player.Instance.level = gameData.level;
            Player.Instance.magic = gameData.magic;
            Player.Instance.maxMagic = gameData.maxMagic;

            isInitialized = true;

            
        }
    }

    private void OnSceneLoaded(Scene scene2, LoadSceneMode mode)
    {
        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Grind":
                GameObject enemyWindow = GameObject.Find(gameData.enemyImageName);

                if (enemyWindow != null)
                {
                    Animator enemySprite = enemyWindow.GetComponent<Animator>();
                    if (enemySprite == null) Debug.LogError("Enemy sprite null! " + enemySprite);
                    if (enemyWindow == null) Debug.LogError("Enemy window null! " + enemyWindow);

                    if (enemySprite != null && gameData.enemySprites.Length > 0)
                    {
                        int enemyIndex = Random.Range(0, gameData.zoneNumEnemies-1);
                        enemySprite.SetInteger("EnemyIndex", enemyIndex);
                    }
                }
                break;
                        
            case "Loot":

                break;
            case "Shop":

                break;
            case "Boss":

                break;

            case "GameOver":

                break;



        }
        // Update any pertinent variables here
    }

    // Update is called once per frame
    void Update()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        if (Player.Instance.hp <= 0 &&  activeScene != "GameOver") 
        {
            StartCoroutine(WaitForLoad());
            if (Player.Instance.hp > 0) return;
            SceneManager.LoadScene("GameOver");
        }

        if (Player.Instance.experience >= Player.Instance.expNextLevel)
        {
            //insert some shoddy math for an experience curve
            Player.Instance.expNextLevel = (int)Mathf.Ceil(Player.Instance.experience * 1.4f * Player.Instance.level);
            Player.Instance.level ++;

            foreach (var kvp in Player.Instance.levelUp)
            { 
                string statName = kvp.Key;
                float statIncrease = kvp.Value;

                UpdateStat(statName, statIncrease);
            }
            GameManager.Instance.onLevelUp.Invoke();
        }
    }

    /*private void SetAspectRatio()
    {
        Camera.main.aspect = 4f / 3f;
    }*/

    private IEnumerator WaitForLoad() 
    {
        yield return null;
    }

    private void HardReset()
    {
        gameData = resetGameData;
        StartCoroutine(WaitForLoad());
        Player.Instance.hp = gameData.hp;
        Player.Instance.maxHP = gameData.maxHP;
        Player.Instance.level = gameData.level;
        Player.Instance.magic = gameData.magic;
        Player.Instance.maxMagic = gameData.maxMagic;
        SceneManager.LoadScene("Main");
    }

    private void UpdateStat(string statName, float statIncrease)
    {
        var field = typeof(Player).GetField(statName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            // Cast the result to float
            float currentValue = (float)field.GetValue(Player.Instance);
            float newValue = currentValue + statIncrease;
            field.SetValue(Player.Instance, newValue);
        }
        else
        {
            Debug.Log($"{statName} is not a valid field.");
        }

    }
}
