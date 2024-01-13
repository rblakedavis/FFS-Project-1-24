using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    GameData gameData;
    Player player;

    private void Awake()
    {
        gameData = GameManager.Instance.gameData;
    }
    private void Start()
    {
        player = Player.Instance;
    }
    public void ChangeScene(string sceneName)
    {
        if (sceneName != "Boss")
        {
            SceneManager.LoadScene(sceneName);
        }
        else if (player.level >= GameManager.Instance.bossRequiredLevel)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
