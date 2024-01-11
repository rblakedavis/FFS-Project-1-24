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
    [SerializeField] private GameObject screenFlash;

    public Animator animator;

    public ScreenShake screenShake;
    private GameData gameData;
    public SceneChanger sceneChanger;

    [SerializeField] float screenShakeDuration;
    [SerializeField] float screenShakeMagnitude;
   // [SerializeField] Color screenFlashColor;
   // [SerializeField] float screenFlashDuration;



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
        healthBar.fillAmount = player.hp / player.maxHP;

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

    public void playerRun()
    {
        gameData.subWindowText = "ran away...";
        sceneChanger.ChangeScene("Main");
    }

    public void EnemyDead() 
    {
        gameData.subWindowText = "Enemy defeated. You lick off the blood on your weapon.";
        sceneChanger.ChangeScene("Main");
    }

    public void playerHeal()
    {
        player.hp += player.healAmount;
        player.magic -= 2;
    }



    private IEnumerator BattleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / enemy.attacksPerSecond);
            player.hp -= enemy.damage;
            player.hp = Mathf.RoundToInt(player.hp);
            Quaternion quaternion = new Quaternion();
            Vector4 rotation = new Vector4(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f), 0);
            quaternion.Set(rotation.x, rotation.y, rotation.z, rotation.w);

            screenFlash.transform.rotation = quaternion;
            screenFlash.transform.localScale = new Vector3(850, 850, 850);



            if (screenShake != null)
            {
                StartCoroutine(screenShake.Shake(screenShakeDuration, screenShakeMagnitude, screenShakeDuration));
            }

            healthTMP.text = player.hp.ToString();

            Debug.Log("Player Health is " + player.hp);
            battleText.text = "You took " + enemy.damage + " damage from " + enemy.enemyName;
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }
}
