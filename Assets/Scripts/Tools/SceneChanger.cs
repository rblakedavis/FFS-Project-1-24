using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    GameData gameData;


    private void Awake()
    {
        gameData = GameManager.Instance.gameData;
    }
    public void ChangeScene(string sceneName)
    {
        if (sceneName != "Boss")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (gameData.level >= GameManager.Instance.bossRequiredLevel)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
