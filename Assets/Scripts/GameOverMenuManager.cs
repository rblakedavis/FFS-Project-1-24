
using UnityEngine;
using TMPro;


public class GameOverMenuManager : MonoBehaviour
{
    public TextMeshProUGUI gameOverMessage;

    public void Awake()
    {
        string[] gameOverMessages = {
            "(Be sure to check the shop when you level up or change zones)",
            "(A strong offence is good, but defense is important too)",
            "(Magic attacks are strongest when your mana is high, wait for the right moment to cast)",
            "(It is best to have full health and mana before fighting the boss)"
        };
        gameOverMessage.text = gameOverMessages[(int)Random.Range(0, gameOverMessages.Length - 1 )];
    }

    public void startAgain()
    {
        GameManager.Instance.BroadcastMessage("HardReset");
    }


    public void quitGame()
    {
        Application.Quit();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            quitGame();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            startAgain();
        }
    }
}