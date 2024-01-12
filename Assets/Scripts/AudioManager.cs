using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip mainMenuMusic;
    public AudioClip lootMenuMusic;
    public AudioClip shopMenuMusic;
    public AudioClip grindMenuMusic;
    public AudioClip gameOverMenuMusic;
    public AudioClip[] bossMenuMusic;
    private Dictionary<string, AudioClip> sceneMusicDictionary = new Dictionary<string, AudioClip>();

    private AudioSource audioSource;


    private static AudioManager _instance;
    private bool isInitialized = false;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(AudioManager).Name);
                    _instance = singleton.AddComponent<AudioManager>();
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

                // Initialize audio manager
                InitializeSceneMusicDictionary();
                isInitialized = true;
            }
        }
    }


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNonBossSceneSpecificMusic("Main");
    }

    private void InitializeSceneMusicDictionary()
    {
        sceneMusicDictionary.Add("Loot", lootMenuMusic);
        sceneMusicDictionary.Add("Shop", shopMenuMusic);
        sceneMusicDictionary.Add("Grind", grindMenuMusic);
        sceneMusicDictionary.Add("GameOver", gameOverMenuMusic);
        sceneMusicDictionary.Add("Main", mainMenuMusic);
    }

    public void PlaySpecificBossMusic(int zone)
    {
        PlayMusic(bossMenuMusic[1]);
    }

    public void PlayNonBossSceneSpecificMusic(string sceneName)
    {
        if (sceneMusicDictionary.ContainsKey(sceneName))
        {
            PlayMusic(sceneMusicDictionary[sceneName]);
        }
        else
        {
            Debug.Log($"No music clip found for scene {sceneName}");
        }
    }
    private void PlayMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

}


