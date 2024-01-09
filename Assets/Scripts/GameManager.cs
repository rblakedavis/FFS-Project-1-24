using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zone;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI magic;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI subWindow;
    [SerializeField] private SceneChanger sceneChanger;

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
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_instance);
        Debug.Log("Awake");

        SetAspectRatio();

        // Handle multiple instances of the GameManager
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            if (isInitialized)
            {
                Debug.LogError("Duplicate GameManager instance detected and initialized. Destroying this instance.");
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("Duplicate GameManager instance detected. Destroying this instance without initialization.");
                Destroy(gameObject);
            }
        }

        if (gameData == null) 
        {
            gameData = ScriptableObject.CreateInstance<GameData>();
            DontDestroyOnLoad(gameData);
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
            level.text = gameData.levelCur.ToString();
            magic.text = gameData.magicCur.ToString();
            health.text = gameData.healthCur.ToString();
            isInitialized = true;
            
        }
    }

    private void OnSceneLoaded(Scene scene2, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Active scene is " + scene.name);
        switch (scene.name)
        {
            case "Grind":
                GameObject enemyWindow = GameObject.Find(gameData.enemyImageName);
                Debug.Log("Entered Grind Case");

                if (enemyWindow != null)
                {
                    Animator enemySprite = enemyWindow.GetComponent<Animator>();
                    if (enemySprite == null) Debug.LogError("Enemy sprite null! " + enemySprite);
                    if (enemyWindow == null) Debug.LogError("Enemy window null! " + enemyWindow);

                    if (enemySprite != null && gameData.enemySprites.Length > 0)
                    {
                        int enemyIndex = Random.Range(0, gameData.zoneNumEnemies-1);
                        Debug.Log("enemyIndex is " + enemyIndex);
                        enemySprite.SetInteger("EnemyIndex", enemyIndex);
                        Debug.Log("Enemy sprite changed successfully.");
                    }
                    else
                    {
                        Debug.Log("Enemy Window " + enemySprite);
                        Debug.Log("Enemy sprite array count " + gameData.enemySprites.Length);


                        Debug.LogError("Image component or sprite array not found.");
                        

                    }
                }
                else
                {
                    Debug.LogError("Enemy Window not found.");
                }
                break;
            
            default:
                Debug.LogWarning("Scene name not in this switch: " + scene.name);
                break;
            
            case "Loot":

                break;
            case "Shop":

                break;
            case "Boss":

                break;
        }
        // Update any pertinent variables here
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Main" && subWindow.text != gameData.subWindowText)
        {
            Debug.Log("Hello World");
            subWindow.text = gameData.subWindowText;
        }
    }

    private void SetAspectRatio()
    {
        Camera.main.aspect = 4f / 3f;
    }

}
