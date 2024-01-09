using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject enemyWindow = GameObject.Find("BaddiesWindow");
        if (enemyWindow != null) 
        {
            Debug.Log("Enemy window is " + enemyWindow);
        }
        else if (enemyWindow == null) 
        {
            Debug.Log("EnemyWindow Not Found");
        }
    }
}
