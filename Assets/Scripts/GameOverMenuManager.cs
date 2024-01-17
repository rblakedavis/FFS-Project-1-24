
using UnityEngine;


public class GameOverMenuManager : MonoBehaviour
{

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