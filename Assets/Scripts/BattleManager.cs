using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class NewBattleManager : MonoBehaviour
{
    public Enemy enemy;

    private Player player;
    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthTMP;

    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private GameObject[] showAndHideEnemyHealth;

    [SerializeField] private Image magicBar;
    [SerializeField] private TextMeshProUGUI magicTMP;

    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject screenFlash;

    public float playerShield = 0;
    public float maxShield;

    public Animator animator;

    //tools
    public ScreenShake screenShake;
    private GameData gameData;
    public SceneChanger sceneChanger;

    [SerializeField] float screenShakeDuration;
    [SerializeField] float screenShakeMagnitude;

    private float minRange;
    private float maxRange;

    private float enemyCooldown;

    private void Awake()
    {
        //clear the battle text window
        battleText.text = string.Empty;

        //setup gamedata and player references
        gameData = GameManager.Instance.gameData;
        player = Player.Instance;


        //setup player shield, health, magic
        maxShield = player.hp / (2 + (player.level / 5));
        healthTMP.text = Mathf.Floor(player.hp).ToString();
        healthBar.fillAmount = player.hp / player.maxHP;
        //magicTMP... 
        //...

        //setup Screen effects
        screenShake = Camera.main.GetComponent<ScreenShake>();
        if (screenShake == null)
        {
            Debug.Log("ScreenShake script not found");
        }

        //setup and create a new enemy
        minRange = gameData.zoneOffset[gameData.curZoneIndex] - .4f;
        Debug.Log("Min range is" + minRange);
        maxRange = minRange + 2.4f; /* use a constant of 2.4 (because of the way range works) here... assumes ALL zones have 3 enemies*/
        Debug.Log("Max range is" + maxRange);

        GameManager.Instance.onLevelUp.AddListener(OnLevelUp);

        enemy = CreateRandomEnemy(minRange, maxRange);
        if (enemy != null)
        {
           enemyCooldown = enemy.attacksPerSecond / 1f;
        }
    }

    private void Start()
    {
        bool isInitialized = false;
        if (!isInitialized)
        {
            battleText.text = $"{enemy.name} appeared!";
            isInitialized = true;
        }

        //call the battle method
    }

    private void Update()
    {
        if (enemy != null)
        {
            if (enemy.curHealth <= 0)
            {
                enemyCooldown = 2.5f;
                EnemyDead();
            }
            else if (enemyCooldown >= 0)
            {
                enemyCooldown -= Time.deltaTime;
            }
            else if (enemyCooldown < 0)
            {
                enemyCooldown = enemy.attacksPerSecond / 1f;
                playerTakeDamage();
            }

            if (enemy.damage <= 0 &&  enemyCooldown <= 1) 
            {
                animator.SetBool("EnemyDead", false);
                animator.SetInteger("EnemyIndex", enemy.index);
                enemy.damage = gameData.enemyList[enemy.index].damage;
                battleText.text = $"{enemy.name} appeared!";
                enemyHealthBar.fillAmount = enemy.curHealth / enemy.maxHealth;
                for (int i = 0; i < showAndHideEnemyHealth.Length; i++)
                {
                    showAndHideEnemyHealth[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
                }

            }
        }
    }

    public void playerAttack()
    {
        if (enemy != null && enemy.damage > 0)
        {
            enemy.curHealth -= player.attack;
            enemyHealthBar.fillAmount = enemy.curHealth / enemy.maxHealth;
        }
    }
    public void playerDefend()
    {
        if (player != null && enemy != null)
        {
            playerShield += Player.Instance.defense;
        }
    }
    public void playerRun()
    {
        gameData.subWindowText = "ran away...";
        sceneChanger.ChangeScene("Main");
    }

    private Enemy CreateRandomEnemy(float minRange, float maxRange)
    {
        int randomEnemy = (int)Mathf.Round(Random.Range(minRange, maxRange));

        

        Enemy enemyBlueprint = gameData.enemyList[randomEnemy];

        Debug.Log("enemy blueprint is "+ enemyBlueprint);
        Debug.Log("random enemy is " + randomEnemy);
        Enemy enemyCopy = ScriptableObject.Instantiate(enemyBlueprint);
        string originalName = enemyCopy.name;
        enemyCopy.name = originalName.Replace("(Clone)", "");

        animator.SetInteger("EnemyIndex", randomEnemy); // 0 is a placeholder for further animations.

        return enemyCopy;
    }

    private void playerTakeDamage()
    {
        float trueDamage = enemy.damage + (enemy.damageModifier * (player.level - 1) / 2);
        if (playerShield > trueDamage)
        {
            playerShield -= trueDamage;
            //play a sound? show a shield graphic?
            // decrease shield bar
        }
        else if (playerShield > 0 && playerShield  <= trueDamage)
        {
            float newDamage = trueDamage - playerShield;
            player.hp -= newDamage;
            playerShield = 0;
            //play a shield break sound? / graphic?
        }
        else if (playerShield <= 0)
        {
            player.hp -= trueDamage;

            Quaternion quaternion = new Quaternion();
            Vector4 rotation = new Vector4(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f), 0);
            quaternion.Set(rotation.x, rotation.y, rotation.z, rotation.w);
            screenFlash.transform.rotation = quaternion;
            screenFlash.transform.localScale = new Vector3(850, 850, 850);
            StartCoroutine(screenShake.Shake(screenShakeDuration, screenShakeMagnitude, screenShakeDuration));

            healthTMP.text = Mathf.Floor(player.hp).ToString();
            battleText.text = $"You took {Mathf.Ceil(trueDamage)} damage from {enemy.name}";
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }

    public void EnemyDead()
    {
        Player.Instance.experience += enemy.expWorth;
        Destroy(enemy); enemy = null;
        for (int i = 0; i < showAndHideEnemyHealth.Length; i++)
        {
            showAndHideEnemyHealth[i].GetComponent<Image>().color -= new Color(0, 0, 0, 1);
        }
        animator.SetBool("EnemyDead", true);
        animator.SetInteger("EnemyIndex", -1);
        battleText.text = gameData.winQuotes[Random.Range(0, gameData.winQuotes.Length - 1)];
        enemy = CreateRandomEnemy(minRange, maxRange);
        enemy.damage = 0;
    }
    

    public void OnLevelUp()
    {
        enemyCooldown = 5f;
        battleText.text = $"Level up! You are now level {Player.Instance.level}! " +
                $"hp and magic restored. " +
                $"attack and defense up! " +
                $"Enemies will also be stronger...";
        player.hp = player.maxHP;
        player.magic = player.maxMagic;
        healthBar.fillAmount = player.hp / player.maxHP;
        healthTMP.text = Mathf.Floor(player.hp).ToString();


        return;
    }

}
