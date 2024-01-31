using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewBattleManager : MonoBehaviour
{
    public Enemy enemy;

    private Player player;

    [SerializeField] private TextMeshProUGUI powerNumber;


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

    [SerializeField] private ParticleSystem shieldParticle;

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
    [SerializeField] private GameObject magicAttack;
    private bool isMagicAttackInitialized = false;
    private float magicAttackCountdown;
    private bool isMagicAttackActive = false;

    private GameObject magicAttackCopy;
   

    private void Awake()
    {
        shieldDecreasedRealizedMultiplier = shieldDecreaseDefenseMultiplier;
        initialShieldCooldown = shieldCooldown;

        battleText.text = string.Empty;

        gameData = GameData.Instance;

        StartCoroutine(WaitForGameData());

        player = Player.Instance;

        UpdateHUD();

        screenShake = Camera.main.GetComponent<ScreenShake>();

        minRange = gameData.zoneOffset[gameData.curZoneIndex] - .5f;
        maxRange = minRange + 2.499f;


        enemy = CreateRandomEnemy(minRange, maxRange);
        if (enemy != null)
        {
            enemyCooldown = enemy.secondsBetweenAttacks / 1f;
        }

        GameManager.Instance.onLevelUp.AddListener(OnLevelUp);
        
    }

    private void Start()
    {
        bool isInitialized = false;
        if (!isInitialized)
        {
            battleText.text = $"{enemy.enemyName} appeared!";
            isInitialized = true;
        }

        UpdateHUD();

    }

    private void Update()
    {
        powerNumber.text = Player.Instance.magicPower.ToString();

        HandleMagicAttacks();

        HandleEnemyAttacks();

        UpdateShield();

        UpdateHUD();


    }

    public void playerAttack()
    {
        if (enemy != null && enemy.damage > 0)
        {
            enemy.curHealth -= player.attack;
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
        enemyCopy.maxHealth = (enemyBlueprint.maxHealth + ((Player.Instance.level - 1) * enemyBlueprint.healthModifier));
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

        else if (playerShield > 0 && playerShield <= trueDamage)
        {
            float newDamage = (trueDamage - playerShield) / 2;
            player.hp -= newDamage;
            playerShield = 0;
            //play a shield break sound? / graphic?
            audioSource.clip = sFXManager.shieldShatterClip[0];
            shieldParticle.Play();
            audioSource.PlayOneShot(audioSource.clip);
            battleText.text = $"Your shield broke and you took {Mathf.Ceil(newDamage)} damage from {enemy.enemyName}";
            shieldBar.fillAmount = playerShield / maxShield;
            shieldBar.color = new Color(shieldBar.color.r, shieldBar.color.g, shieldBar.color.b, 0);
            shieldIcon.color = new Color(0, 0, 0, 0);
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

    private void UpdateShield()
    {
        if (shieldCooldown > 0)
        {
            shieldCooldown -= Time.deltaTime;
        }
        else if (shieldCooldown <= 0 && playerShield > 0)
        {
            playerShield -= (shieldDecrease * Player.Instance.defense / shieldDecreasedRealizedMultiplier);
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
    }

    private void HandleEnemyAttacks()
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

            if (enemy.damage <= 0 && enemyCooldown <= 1)
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
    }

    private void UpdateHUD()
    {
        healthBar.fillAmount = player.hp / player.maxHP;
        healthTMP.text = Mathf.Floor(player.hp).ToString();
        magicBar.fillAmount = player.magic / player.maxMagic;
        magicTMP.text = Mathf.Floor(player.magic).ToString();
        playerLevelHUD.text = player.level.ToString();
        playerExpToNextHUD.text = (player.expNextLevel - player.experience).ToString();
        maxShield = ((player.hp / 2) + (player.level / 2));
        if (enemy != null)
        {
            enemyHealthBar.fillAmount = enemy.curHealth / enemy.maxHealth;
        }


    }

    private void HandleMagicAttacks()
    {
        if (Player.Instance.magic >= 1)
        {
            if(!isMagicAttackInitialized && !isMagicAttackActive)
            {
                magicAttackCountdown = 1f;
                isMagicAttackInitialized = true;
            }
            else if(isMagicAttackInitialized)
            {
                magicAttackCountdown -= Time.deltaTime;
                if(magicAttackCountdown <= 0)
                {
                    magicAttackCountdown = float.PositiveInfinity;
                    SpawnMagicAttack();
                }
            }
        }
    }

    private void SpawnMagicAttack()
    {
        if (!isMagicAttackActive)
        {
            float randomX = Random.Range(-70f, 70f);
            float randomY = Random.Range(-50f, 50f);
            GameObject magicAttackInstance = Instantiate(magicAttack,new Vector3(randomX, randomY, 0), Quaternion.identity);
            magicAttackInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);

            Rigidbody2D magicAttackRb = magicAttackInstance.GetComponent<Rigidbody2D>();
            float velocityX = Random.Range(Random.Range(90f, 140f), Random.Range(-90f, -140f));
            float velocityY = Random.Range(85f, 125f);
            magicAttackRb.velocity = new Vector2(velocityX, velocityY);
            magicAttackCopy = magicAttackInstance;

            AudioClip clip = sFXManager.magicSpawnedSounds[0];
            audioSource.PlayOneShot(clip);
           
            isMagicAttackActive = true;
        }
    }

    public void DespawnMagicAttack()
    {
        magicAttackCopy = null;
        isMagicAttackActive = false;
        isMagicAttackInitialized = false;
    }
}