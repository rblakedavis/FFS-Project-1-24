using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBattleManager : MonoBehaviour
{
    public Enemy enemy;

    private Player player;


    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI healthTMP;
   
    [SerializeField] private Image shieldBar;
    [SerializeField] private Image shieldIcon;

    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private GameObject[] showAndHideEnemyHealth;

    [SerializeField] private Image magicBar;
    [SerializeField] private TextMeshProUGUI magicTMP;

    [SerializeField] private TextMeshProUGUI battleText;
    [SerializeField] private GameObject screenFlash;

    [HideInInspector] public float playerShield = 0;
    [HideInInspector] public float maxShield;

    public GameObject enemySprite;
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
    private float textCooldown;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private float shieldDecrease;
    [SerializeField] private float shieldDecreaseDefenseMultiplier;
    private float initialShieldCooldown;
    private float shieldDecreasedRealizedMultiplier;
    private float shieldDecreaseAcceleration = 0f;
    [SerializeField] private float shieldDecreaseAccelerationClamp = 5f;
    [SerializeField] private float enemyDeadCooldown;
    [SerializeField] private TextMeshProUGUI playerLevelHUD;
    [SerializeField] private TextMeshProUGUI playerExpToNextHUD;

    [SerializeField] SFXManager sFXManager;
    [SerializeField] AudioSource audioSource;

    [SerializeField] VFXManager vFXManager;

    private void Awake()
    {
        

        shieldDecreasedRealizedMultiplier = shieldDecreaseDefenseMultiplier;
        initialShieldCooldown = shieldCooldown;
        //clear the battle text window
        battleText.text = string.Empty;

        //setup gamedata and player references
        gameData = GameData.Instance;

        StartCoroutine(WaitForGameData());

        player = Player.Instance;


        //setup player shield, health, magic
        maxShield = ((player.hp / 2) + (player.level / 2));
        healthTMP.text = Mathf.Floor(player.hp).ToString();
        healthBar.fillAmount = player.hp / player.maxHP;
        magicTMP.text = Mathf.Floor(player.magic).ToString();
        magicBar.fillAmount = player.magic / player.maxMagic;

        //setup Screen effects
        screenShake = Camera.main.GetComponent<ScreenShake>();
        if (screenShake == null)
        {
            Debug.Log("ScreenShake script not found");
        }

        //setup and create a new enemy
        minRange = gameData.zoneOffset[gameData.curZoneIndex] - .5f;
        Debug.Log("Min range is" + minRange);
        maxRange = minRange + 2.499f; /* use a constant near 2.5 (because of the way range works) here... assumes ALL zones have 3 enemies*/
        Debug.Log("Max range is" + maxRange);

        GameManager.Instance.onLevelUp.AddListener(OnLevelUp);

        enemy = CreateRandomEnemy(minRange, maxRange);
        if (enemy != null)
        {
           enemyCooldown = enemy.secondsBetweenAttacks / 1f;
        }
    }

    private void Start()
    {
        bool isInitialized = false;
        if (!isInitialized)
        {
            battleText.text = $"{enemy.enemyName} appeared!";
            isInitialized = true;
        }



        //call the battle method
        playerLevelHUD.text = player.level.ToString();
        playerExpToNextHUD.text = (player.expNextLevel - player.experience).ToString();
    }

    private void Update()
    {
        if (enemy != null)
        {
            if (enemy.curHealth <= 0)
            {
                enemyCooldown = enemyDeadCooldown;
                EnemyDead();
            }
            else if (enemyCooldown > 0)
            {
                enemyCooldown -= Time.deltaTime;
            }
            else if (enemyCooldown <= 0)
            {
                enemyCooldown = enemy.secondsBetweenAttacks / 1f;
                PlayerTakeDamage();
            }

            if (enemy.damage <= 0 &&  enemyCooldown <= 1) 
            {
                animator.SetBool("EnemyDead", false);
                animator.SetInteger("EnemyIndex", enemy.index);
                enemy.damage = gameData.enemyList[enemy.index].damage;
                battleText.text = $"{enemy.enemyName} appeared!";
                enemyHealthBar.fillAmount = enemy.curHealth / enemy.maxHealth;
                for (int i = 0; i < showAndHideEnemyHealth.Length; i++)
                {
                    showAndHideEnemyHealth[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
                }

            }
        }
        textCooldown = enemyCooldown;
        if (textCooldown <= 0.5f)
        {
            battleText.text = string.Empty;
        }

        if (shieldCooldown > 0)
        {
            shieldCooldown -= Time.deltaTime;
        }
        else if (shieldCooldown <= 0 && playerShield > 0)
        {
            playerShield -= (shieldDecrease * Player.Instance.defense/shieldDecreasedRealizedMultiplier);
            shieldCooldown = initialShieldCooldown;
            shieldBar.fillAmount = playerShield / maxShield;
            if (shieldDecreaseAcceleration <= 0)
            {
                shieldDecreaseAcceleration = 1;
                shieldDecreasedRealizedMultiplier = shieldDecreaseDefenseMultiplier;
            }
            else if (shieldDecreaseAcceleration > shieldDecreaseAccelerationClamp)
            {
                shieldDecreaseAcceleration = shieldDecreaseAccelerationClamp;
            }
            else if (shieldDecreaseAcceleration <= shieldDecreaseAccelerationClamp)
            {
                shieldDecreasedRealizedMultiplier = shieldDecreaseDefenseMultiplier / shieldDecreaseAcceleration;
                shieldDecreaseAcceleration++;
            }

            if (playerShield <= 0)
            {
                shieldBar.fillAmount = playerShield / maxShield;
                shieldBar.color = new Color(shieldBar.color.r, shieldBar.color.g, shieldBar.color.b, 0);
                shieldIcon.color = new Color(0, 0, 0, 0);
                shieldDecreasedRealizedMultiplier = shieldDecreaseDefenseMultiplier;
            }
        }

        healthBar.fillAmount = player.hp / player.maxHP;
        healthTMP.text = Mathf.Floor(player.hp).ToString();
        magicBar.fillAmount = player.magic / player.maxMagic;
        magicTMP.text = Mathf.Floor(player.magic).ToString();
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
            if (playerShield > maxShield)
            {
                playerShield = maxShield;
            }
            shieldBar.fillAmount = playerShield / maxShield;
            shieldBar.color = new Color(1, 1, 1, 0.82f);
            shieldIcon.color = Color.white;
            shieldDecreaseAcceleration = 0;
            Debug.Log("Shield is " + playerShield);
        }
    }
    public void playerRun()
    {
        gameData.subWindowText = "ran away...";
        sceneChanger.ChangeScene("Main");
    }

    private Enemy CreateRandomEnemy(float minRange, float maxRange)
    {
        float randomNumber = Random.Range(minRange, maxRange);
        int randomEnemy = (int)Mathf.Round(randomNumber);


        Enemy enemyBlueprint = gameData.enemyList[randomEnemy];

        Enemy enemyCopy = ScriptableObject.Instantiate(enemyBlueprint);
        string originalName = enemyCopy.name;
        enemyCopy.name = originalName.Replace("(Clone)", "");

        animator.SetInteger("EnemyIndex", randomEnemy);
        enemyCopy.maxHealth = ( enemyBlueprint.maxHealth + (( Player.Instance.level - 1 ) * enemyBlueprint.healthModifier ));
        enemyCopy.curHealth = enemyCopy.maxHealth;

        return enemyCopy;
    }

    private void PlayerTakeDamage()
    {
        float trueDamage = enemy.damage + (enemy.damageModifier * (player.level - 1) / 2);
        if (playerShield > trueDamage)
        {
            playerShield -= trueDamage;
            //play a sound? show a shield graphic?
            int shieldHitSound = Mathf.CeilToInt(Random.Range(0, sFXManager.shieldHitClips.Length));
            audioSource.clip = sFXManager.shieldHitClips[shieldHitSound];
            audioSource.PlayOneShot(audioSource.clip);
            shieldBar.fillAmount = playerShield / maxShield;
            vFXManager.ShieldDown();
            battleText.text = $"You blocked {Mathf.Ceil(trueDamage)} damage with your shield";


        }

        else if (playerShield > 0 && playerShield  <= trueDamage)
        {
            float newDamage = (trueDamage - playerShield) / 2;
            player.hp -= newDamage;
            playerShield = 0;
            //play a shield break sound? / graphic?
            audioSource.clip = sFXManager.shieldShatterClip[0];
            audioSource.PlayOneShot(audioSource.clip);
            battleText.text = $"Your shield broke and you took {Mathf.Ceil(newDamage)} damage from {enemy.enemyName}";
            shieldBar.fillAmount = playerShield / maxShield;
            shieldBar.color = new Color(shieldBar.color.r, shieldBar.color.g, shieldBar.color.b, 0);
            shieldIcon.color = new Color (0, 0, 0, 0);
        }

        else if (playerShield <= 0)
        {
            player.hp -= trueDamage;

            screenFlash.transform.eulerAngles = new Vector3(Random.Range(-360f, 360f), Random.Range(-360f, 360f), Random.Range(-360f, 360f));

            screenFlash.transform.localScale = new Vector3(850, 850, 850);
            StartCoroutine(screenShake.Shake(screenShakeDuration, screenShakeMagnitude, screenShakeDuration));
            audioSource.clip = sFXManager.unshieldedHitClips[0];
            audioSource.PlayOneShot(audioSource.clip);
            healthTMP.text = Mathf.Floor(player.hp).ToString();
            //vFXManager.HealthDown();
            battleText.text = $"You took {Mathf.Ceil(trueDamage)} damage from {enemy.enemyName}";
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }

    public void EnemyDead()
    {
        audioSource.clip = sFXManager.enemyDownClips[0];
        audioSource.PlayOneShot(audioSource.clip);
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
        playerExpToNextHUD.text = (player.expNextLevel - player.experience).ToString();
    }
    

    public void OnLevelUp()
    {
        enemyCooldown = 5.5f;
        battleText.text = $"Lv up! You are Lv {Player.Instance.level}! \n" +
                $"Hp and Mag up! \n" +
                $"Atk is {Mathf.Ceil(player.attack)}, Def is {Mathf.Ceil(player.defense)} \n \n" +
                $"Enemies got stronger...";
        player.hp = player.maxHP;
        player.magic = player.maxMagic;
        playerLevelHUD.text = player.level.ToString();
        playerExpToNextHUD.text = (player.expNextLevel - player.experience).ToString();




        return;
    }

    private IEnumerator WaitForGameData()
    {
        if (gameData != GameData.Instance)
        {
            yield return new WaitForEndOfFrame();
            gameData = GameData.Instance;
            Awake();
        }
        else
        {
            StopAllCoroutines();
        }
    }

}
