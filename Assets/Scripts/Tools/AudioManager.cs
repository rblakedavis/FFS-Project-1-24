using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip gameOverMenuMusic;
    public AudioClip[] bossMenuMusic;

    public AudioClip forestMusic;
    public AudioClip caveMusic;
    public AudioClip ruinsMusic;
    public AudioClip depthsMusic;
    public AudioClip underworldMusic;

    private Dictionary<string, AudioClip> sceneMusicDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<int, AudioClip> zoneMusicDictionary = new Dictionary<int, AudioClip>();

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
        DontDestroyOnLoad(gameObject);
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
            else
            {
                Destroy(gameObject);
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
        sceneMusicDictionary.Add("GameOver", gameOverMenuMusic);

        zoneMusicDictionary.Add(0, forestMusic);
        zoneMusicDictionary.Add(1, caveMusic);
        zoneMusicDictionary.Add(2, ruinsMusic);
        zoneMusicDictionary.Add(3, depthsMusic);
        zoneMusicDictionary.Add(4, underworldMusic);
    }

    public void PlaySpecificBossMusic(int zone)
    {
        PlayMusic(bossMenuMusic[zone]);
    }

    public void PlayNonBossSceneSpecificMusic(string sceneName)
    {
        if (sceneName == "GameOver")
        {
            PlayMusic(gameOverMenuMusic);
        }
        else
        {
            PlayMusic(zoneMusicDictionary[GameData.Instance.curZoneIndex]);
        }
    }
    private void PlayMusic(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

}


