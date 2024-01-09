using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(GameManager).Name);
                    _instance = singleton.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!isInitialized)
        {

            Debug.Log("Start");
            Data.subWindowText = "Welcome to the game.";
            subWindow.text = Data.subWindowText;
            zone.text=Data.zoneNames[0];
            level.text = Data.levelCur.ToString();
            magic.text = Data.magicCur.ToString();
            health.text = Data.healthCur.ToString();
            
        }
    }

    private void OnSceneLoaded()
    {
        //update any pertinent variables here
    } 

    // Update is called once per frame
    void Update()
    {

        Camera.main.aspect = 4f / 3f;


        if (subWindow.text != Data.subWindowText)
            {
                subWindow.text = Data.subWindowText;
            }
    }
}
