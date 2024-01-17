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
    [SerializeField] public TextMeshProUGUI subWindow;

    [SerializeField] private Image healthBar;
    private float regenCooldown = .5f;
    private float timeSinceRegen = 0f;
    [SerializeField] private Image magicBar;
    private bool isMainMenuMusicPlaying = true;

    [SerializeField]private GameData resetGameData;

    public UnityEvent onLevelUp;
    public int bossRequiredLevel;

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
        if (Player.Instance != null)
        {
            bossRequiredLevel = (1 + GameData.Instance.curZoneIndex) * 5;
        }

        StartCoroutine(WaitForLoad());

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_instance);

        if (_instance != this)
        {
            if (_instance.isInitialized)
            {
                DestroyImmediate(_instance.transform.gameObject);
                _instance = this;
                isInitialized = true;

            }
            else { DestroyImmediate(this); Debug.Log("Destroying this..."); }


        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    

    private void OnDestroy()
    {
        Debug.Log("Destroyed");
    }

    private void OnEnable()
    {
        gameData = GameData.Instance;
        gameData.subWindowText = $"Welcome to the {gameData.zoneNames[gameData.curZoneIndex]}.";


        Debug.Log("OnEnable");
        if (!isInitialized && SceneManager.GetActiveScene().name == "Main")
        {
            gameData = GameData.Instance;
            
            subWindow.text = gameData.subWindowText;
            zone.text = gameData.zoneNames[gameData.curZoneIndex];
            level.text = Player.Instance.level.ToString();
            magic.text = Mathf.Floor(Player.Instance.magic).ToString();
            health.text = Mathf.Floor(Player.Instance.hp).ToString();

            isInitialized = true;           
        }
    }

    #region Handle scene loads
    private void OnSceneLoaded(Scene scene2, LoadSceneMode mode)
    {
        

        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Main":
                subWindow = GameObject.Find("NotifWindowText").GetComponentInChildren<TextMeshProUGUI>();
                if (subWindow !=null)
                { subWindow.text = string.Empty; }
                
                if (!isMainMenuMusicPlaying)
                {
                    AudioManager.Instance.PlayNonBossSceneSpecificMusic(scene.name);
                    isMainMenuMusicPlaying = false;
                }
                break;
            case "Grind":

                break;
                        
            case "Loot":

                break;

            case "Shop":

                break;
            case "Boss":
                AudioManager.Instance.PlaySpecificBossMusic(GameData.Instance.curZoneIndex);
                isMainMenuMusicPlaying = false;
                break;

            case "GameOver":
                AudioManager.Instance.PlayNonBossSceneSpecificMusic(scene.name);
                isMainMenuMusicPlaying = false;
                Player.Instance.hp = Player.Instance.maxHP;
                break;
        }
        // Update any pertinent variables here
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (regenCooldown > timeSinceRegen)
        {
            timeSinceRegen += Time.deltaTime;
        }
        if (timeSinceRegen > regenCooldown && Player.Instance.hp < Player.Instance.maxHP) 
        {
            Player.Instance.hp += Player.Instance.hpRegen;
            timeSinceRegen = 0;
            if (SceneManager.GetActiveScene().name == "Main")
            {
                healthBar.fillAmount = Player.Instance.hp / Player.Instance.maxHP;
                health.text = Mathf.Floor(Player.Instance.hp).ToString();
            }

        }
        
        if (timeSinceRegen > regenCooldown && Player.Instance.magic < Player.Instance.maxMagic)
        {
            Player.Instance.magic += Player.Instance.magicRegen;
            timeSinceRegen = 0;
            if (SceneManager.GetActiveScene().name == "Main")
            {
                magicBar.fillAmount = Player.Instance.magic / Player.Instance.maxMagic;
                magic.text = Mathf.Floor(Player.Instance.magic).ToString();
                
            }

        }

        string activeScene = SceneManager.GetActiveScene().name;
        if (Player.Instance.hp <= 0 && activeScene != "GameOver")
        {
            StartCoroutine(WaitForLoad());
            if (Player.Instance.hp > 0) return;
            SceneManager.LoadScene("GameOver");
        }

        if (Player.Instance.experience >= Player.Instance.expNextLevel)
        {
            //insert some shoddy math for an experience curve
            Player.Instance.expNextLevel = (int)Mathf.Ceil((2f * Player.Instance.experience) + (1.4f * Player.Instance.level));
            Player.Instance.level++;

            foreach (var kvp in Player.Instance.levelUp)
            {
                string statName = kvp.Key;
                float statIncrease = kvp.Value;

                UpdateStat(statName, statIncrease);
            }
            bossRequiredLevel = (1 + GameData.Instance.curZoneIndex) * 5;
            GameManager.Instance.onLevelUp.Invoke();
        }
        if (SceneManager.GetActiveScene().name == "Main" && subWindow.text != GameData.Instance.subWindowText)
        {
            subWindow.text = GameData.Instance.subWindowText;
            if (GameData.Instance.subWindowText == "")
            {
                GameData.Instance.subWindowText = $"Welcome to the {gameData.zoneNames[gameData.curZoneIndex]}.";
            }
            zone.text = GameData.Instance.zoneNames[GameData.Instance.curZoneIndex];
            level.text = Player.Instance.level.ToString();
            magic.text = Mathf.Floor(Player.Instance.magic).ToString();
            health.text = Mathf.Floor(Player.Instance.hp).ToString();
            healthBar.fillAmount = Player.Instance.hp / Player.Instance.maxHP;
            magicBar.fillAmount = Player.Instance.magic / Player.Instance.maxMagic;


        }

        if (Player.Instance.hp > Player.Instance.maxHP) 
        {
            Player.Instance.hp = Player.Instance.maxHP;
        }
    }

        private IEnumerator WaitForLoad() 
    {
        yield return null;
    }

    private void HardReset()
    {
        gameData = resetGameData;
        StartCoroutine(WaitForLoad());
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
