using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public Enemy enemy;
    private Player player;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private TextMeshProUGUI healthTMP;

    [SerializeField] private Image magicBar;
    [SerializeField] private TextMeshProUGUI magicTMP;

    [SerializeField] private TextMeshProUGUI battleText;
    public Animator animator;

    public ScreenShake screenShake;
    public GameData gameData;


    void Awake()
    {
        battleText.text = string.Empty;

        gameData = GameManager.Instance.gameData;

        player = Player.Instance;
        healthTMP.text = player.hp.ToString();



        screenShake = Camera.main.GetComponent<ScreenShake>();
        if (screenShake == null)
        {
            Debug.LogError("ScreenShake script not found");
        }
    }

    void Start()
    {
        StartCoroutine(BattleCoroutine());
        enemy.health = 15;
        enemy.curHealth = 15;

    }

    void Update()
    {
        
    }

    public void playerAttack()
    {
        if (player != null && enemy != null)
        {
            enemy.curHealth -= player.attack;
            enemyHealthBar.fillAmount = enemy.curHealth / enemy.health;
        }
    }



    private IEnumerator BattleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / enemy.attacksPerSecond);
            player.hp -= enemy.damage;

            if (screenShake != null)
            {
                StartCoroutine(screenShake.Shake(.1f, 1.5f));
            }

            healthTMP.text = player.hp.ToString();

            Debug.Log("Player Health is " + player.hp);
            battleText.text = "You took " + enemy.damage + " damage from " + enemy.enemyName;
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }
}
