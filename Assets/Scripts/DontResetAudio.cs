using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Import this to use SceneManager

public class DontResetAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "shop" || sceneName == "main" || sceneName == "loot")
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}