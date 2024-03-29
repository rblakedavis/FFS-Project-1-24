using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class BossBattleManager : MonoBehaviour
{

    [SerializeField] private Item blobbyKeyItem;
    [SerializeField] private Item caveTrollKeyItem;
    [SerializeField] private Item minotaurKeyItem;
    [SerializeField] private Item deepHorrorKeyItem;
    [SerializeField] private Item gateKeeperKeyItem;

    [SerializeField] private TextMeshProUGUI powerNumber;

    [SerializeField] private ParticleSystem shieldParticle;

    public Enemy enemy;
    private bool isEnemyDead = false;
    private bool playerHasChangedZones = false;

    [SerializeField] private float bossDeadDelay;
    private float bossDeadElapsed = 0f;

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

    public float playerShield = 0;
    public float maxShield;

    public Animator animator;

    //tools
    public ScreenShake screenShake;
    private GameData gameData;
    public SceneChanger sceneChanger;

    [SerializeField] float screenShakeDuration;
    [SerializeField] float screenShakeMagnitude;



    private float enemyCooldown;
    private float textCooldown;
    [SerializeField] private float shieldCooldown;
    [SerializeField] private float shieldDecrease;
    [SerializeField] private float shieldDecreaseDefenseMultiplier;
    private float initialShieldCooldown;
    private float shieldDecreasedRealizedMultiplier;
    private float shieldDecreaseAcceleration = 0f;
    [SerializeField] private float shieldDecreaseAccelerationClamp;

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

        //clear the battle text window
        battleText.text = string.Empty;

        //setup gamedata and player references
        gameData = GameData.Instance;
        player = Player.Instance;


        //setup player shield, health, magic


        //setup Screen effects
        screenShake = Camera.main.GetComponent<ScreenShake>();
        playerHasChangedZones = false;



        //GameManager.Instance.onLevelUp.AddListener(OnLevelUp);

        enemy = CreateBoss(gameData.curZoneIndex);
        switch (enemy.enemyName)
        {
            case "Blobby":
                if (Player.Instance.HasItem(blobbyKeyItem))
                {
                    enemy.damage = enemy.damage / 20;
                }
                break;
            case "Cave Troll":
                if (Player.Instance.HasItem(caveTrollKeyItem))
                {
                    enemy.damage = enemy.damage / 20;
                }
                break;

            case "Minotaur":
                if (Player.Instance.HasItem(minotaurKeyItem))
                {
                    enemy.damage = enemy.damage / 20;
                }
                break;

            case "Deep Horror":
                if (Player.Instance.HasItem(deepHorrorKeyItem))
                {
                    enemy.damage = enemy.damage / 20;
                }
                break;

            case "Gatekeeper":
                if (Player.Instance.HasItem(gateKeeperKeyItem))
                {
                    enemy.damage = enemy.damage / 20;
                }
                break;

            default:

                Debug.LogError("Boss not found in list!");
                break;


        }
        if (enemy != null)
        {
            enemyCooldown = enemy.secondsBetweenAttacks / 1f;
        }
    }

    private void Start()
    {
        battleText.text = $"You foolishly challenge {enemy.enemyName}...";
        playerLevelHUD.text = player.level.ToString();
        playerExpToNextHUD.text = (player.expNextLevel - player.experience).ToString();

    }

    private void Update()
    {
        powerNumber.text = Player.Instance.magicPower.ToString();

        HandleEnemyAttacks();

        HandleBossDeath();
        
        UpdateHUD();

        HandleMagicAttacks();
        
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
        }
    }

    private Enemy CreateBoss(int zoneIndex)
    {
        Enemy enemyBlueprint = gameData.bossList[zoneIndex];
        Enemy enemyCopy = ScriptableObject.Instantiate(enemyBlueprint);
        string originalName = enemyCopy.name;
        enemyCopy.name = originalName.Replace("(Clone)", "");

        animator.SetInteger("BossIndex", zoneIndex);

        return enemyCopy;
    }

    private void PlayerTakeDamage()
    {
        float trueDamage = enemy.damage + (enemy.damageModifier * (player.level - 1) / 2);
        if (playerShield > trueDamage)
        {
            playerShield -= trueDamage;
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
            audioSource.clip = sFXManager.shieldShatterClip[0];
            audioSource.PlayOneShot(audioSource.clip);
            shieldParticle.Play();
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
            battleText.text = $"You took {Mathf.Ceil(trueDamage)} damage from {enemy.enemyName}";
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }

    public void EnemyDead()
    {
        audioSource.clip = sFXManager.enemyDownClips[0];
        audioSource.PlayOneShot(audioSource.clip);
        Destroy(enemy); enemy = null;
        for (int i = 0; i < showAndHideEnemyHealth.Length; i++)
        {
            showAndHideEnemyHealth[i].GetComponent<Image>().color -= new Color(0, 0, 0, 1);
        }
        animator.SetBool("EnemyDead", true);
        animator.SetInteger("EnemyIndex", -1);
        battleText.text = gameData.winQuotes[Random.Range(0, gameData.winQuotes.Length - 1)];
        isEnemyDead = true;

    }


    private void HandleEnemyAttacks()
    {
        if (enemy != null)
        {
            if (enemy.curHealth <= 0)
            {
                enemyCooldown = 2.5f;
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

        }

        textCooldown = enemyCooldown;

        if (textCooldown <= 0.5f)
        {
            battleText.text = string.Empty;
        }

    }

    private void HandleBossDeath()
    {
        if (isEnemyDead && bossDeadElapsed < bossDeadDelay)
        {
            bossDeadElapsed += Time.deltaTime;
        }

        if (bossDeadElapsed >= bossDeadDelay / 2 && !playerHasChangedZones && gameData.curZoneIndex < 4)
        {
            battleText.text = $"Left {gameData.zoneNames[gameData.curZoneIndex]}\n";
            gameData.curZoneIndex++;
            battleText.text += $"Moving to {gameData.zoneNames[gameData.curZoneIndex]}";
            Player.Instance.hp = Player.Instance.maxHP;
            playerHasChangedZones = true;
        }
        else if (bossDeadElapsed >= bossDeadDelay / 2 && !playerHasChangedZones && gameData.curZoneIndex >= 4)
        {
            sceneChanger.ChangeScene("GameEnd");
        }

        if (bossDeadElapsed >= bossDeadDelay)
        {
            sceneChanger.ChangeScene("Main");
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

    private void UpdateHUD()
    {
        UpdateShield();
        maxShield = ((player.hp / 2) + (player.level / 2));
        healthTMP.text = Mathf.Floor(player.hp).ToString();
        healthBar.fillAmount = player.hp / player.maxHP;
        magicTMP.text = Mathf.Floor(player.magic).ToString();
        magicBar.fillAmount = player.magic / player.maxMagic;
        if (enemy != null)
        {
            enemyHealthBar.fillAmount = enemy.curHealth / enemy.maxHealth;
        }
    }

    private void HandleMagicAttacks()
    {
        if (Player.Instance.magic >= 1)
        {
            if (!isMagicAttackInitialized && !isMagicAttackActive)
            {
                magicAttackCountdown = 1f;
                isMagicAttackInitialized = true;
            }
            else if (isMagicAttackInitialized)
            {
                magicAttackCountdown -= Time.deltaTime;
                if (magicAttackCountdown <= 0)
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
            GameObject magicAttackInstance = Instantiate(magicAttack, new Vector3(randomX, randomY, 0), Quaternion.identity);
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