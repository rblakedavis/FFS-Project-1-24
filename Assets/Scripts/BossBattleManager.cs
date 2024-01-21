using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class BossBattleManager : MonoBehaviour
{
    [SerializeField] private Item blobbyKeyItem;

    public Enemy enemy;
    private bool isEnemyDead = false;
    private bool playerHasChangedZones = false;

    [SerializeField]private float bossDeadDelay;
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
                //ifplayerhassecretitem....
                break;

            case "Minotaur":
                break;

            case "Deep Horror":
                break;

            case "Gatekeeper":
                break;

            default:

                Debug.LogError("Boss not found in list!");
                break;
            // Pseudocode: 
            // case caveboss: 
            // check for caveboss key item
            // etc... 
            
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

        if (isEnemyDead && bossDeadElapsed < bossDeadDelay)
        {
            bossDeadElapsed += Time.deltaTime;
        }

        if (bossDeadElapsed >= bossDeadDelay/2 && !playerHasChangedZones)
        {
            battleText.text = $"Left {gameData.zoneNames[gameData.curZoneIndex]}\n";
            gameData.curZoneIndex++;
            //if curZoneIndex >= 4{}....
            battleText.text += $"Moving to {gameData.zoneNames[gameData.curZoneIndex]}";
            playerHasChangedZones = true;
        }

        if (bossDeadElapsed >= bossDeadDelay)
        {
            sceneChanger.ChangeScene("Main");
        }

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
    

    /*
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
    */

}
