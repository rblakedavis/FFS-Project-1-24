
using UnityEngine;

public class GameOverMenuManager : MonoBehaviour
{
    [SerializeField] private SceneChanger sceneChanger;

    public void startAgain()
    {
        GameManager.Instance.BroadcastMessage("HardReset");
    }

    public void quitGame()
    {
        Application.Quit();
    }
}