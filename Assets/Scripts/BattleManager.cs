using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{
    public Enemy enemy;

    private Player player;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image enemyHealthBar;
    [SerializeField] private GameObject[] enemyHealthGroup;
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
    private int minRange;
    private int maxRange;

    public float playerShield = 0;
    public float maxShield;

    private bool isAnyKeyDown;

    private bool spawnEnemies = true;

    private float timeElapsed;

    [SerializeField] private Image enemyImage;


    void Awake()
    {
        gameData = GameManager.Instance.gameData;
        player = Player.Instance;

        maxShield = player.hp / ( 2 + ( player.level / 5 ));
        battleText.text = string.Empty;

        gameData = GameManager.Instance.gameData;

        healthTMP.text = Mathf.Floor(player.hp).ToString();



        screenShake = Camera.main.GetComponent<ScreenShake>();
        if (screenShake == null)
        {
            Debug.LogError("ScreenShake script not found");
        }
        healthBar.fillAmount = player.hp / player.maxHP;


        //create a random enemy based on the current zone.
        // PLACEHOLDER = use a constant of 2
        // assumes all zones have 3 enemies
        minRange = gameData.zoneOffset[gameData.curZoneIndex];
        maxRange = minRange + 2;
        enemy = CreateRandomEnemy(minRange, maxRange);

        GameManager.Instance.onLevelUp.AddListener(OnLevelUp);
    }

    void Start()
    {
        StartCoroutine(BattleCoroutine());
        // placeholder stats for proper implementation

    }

    void Update()
    {
        isAnyKeyDown = Input.anyKeyDown;
        if (enemy != null && enemy.curHealth <= 0)
        {
            StopCoroutine(BattleCoroutine());
            StartCoroutine(EnemyDead());
        }
        if (enemy == null && spawnEnemies )
        { 
            CreateRandomEnemy(minRange, maxRange); 
            if (enemy != null)
            {
                StartCoroutine(BattleCoroutine());
            }
        }

    }

    public void playerAttack()
    {
        if (player != null && enemy != null)
        {
                enemy.curHealth -= player.attack;
                enemyHealthBar.fillAmount = enemy.curHealth / enemy.health;
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



private IEnumerator BattleCoroutine()
{
    while (enemy != null && enemy.damage > 0)
    {
        yield return new WaitForSeconds(1f / enemy.attacksPerSecond);
        if (enemy.damage > 0)
        {
            float trueDamage = enemy.damage + (enemy.damageModifier * (player.level - 1));
            player.hp -= trueDamage;

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

            battleText.text = "You took " + trueDamage + " damage from " + enemy.name;
            healthBar.fillAmount = player.hp / player.maxHP;
        }
    }
}

    private Enemy CreateRandomEnemy(int rangeMin, int rangeMax)
    {
        if (spawnEnemies)
        {
            StopCoroutine(LevelUpCoroutine());

            int randomEnemy = Random.Range(rangeMin, rangeMax);
            Enemy enemyBlueprint = gameData.enemyList[randomEnemy];
            Enemy enemyOut = ScriptableObject.CreateInstance<Enemy>();
            enemyOut.name = enemyBlueprint.name;
            enemyOut.damage = enemyBlueprint.damage;
            enemyOut.damageModifier = enemyBlueprint.damageModifier;
            enemyOut.health = enemyBlueprint.health;
            enemyOut.curHealth = enemyBlueprint.curHealth;
            enemyOut.expWorth = enemyBlueprint.expWorth;
            enemyOut.attacksPerSecond = enemyBlueprint.attacksPerSecond;
            enemyOut.sprite = enemyBlueprint.sprite;
            enemyImage.sprite = enemyOut.sprite;
            enemyHealthBar.fillAmount = enemyOut.curHealth / enemyOut.health;


            return enemyOut;
        }
        else return null;
    }

    public IEnumerator EnemyDead()
    {
        spawnEnemies = false;
        Player.Instance.experience += enemy.expWorth;
        for (int i = 0; i < enemyHealthGroup.Length; i++)
        {
        enemyHealthGroup[i].GetComponent<Image>().color -= new Color(0, 0, 0, 1);
        }
        Destroy(enemy); enemy = null;
        animator.SetBool("EnemyDead", true);
        animator.SetInteger("EnemyIndex", -1);
        battleText.text = gameData.winQuotes[Random.Range(0, gameData.winQuotes.Length - 1)];
        spawnEnemies = true;
        enemy = CreateRandomEnemy(minRange, maxRange);
        if (enemy != null)
        {
            float storeEneDam = enemy.damage;
            enemy.damage = 0;

            yield return new WaitForSeconds(1.5f);

            animator.SetBool("EnemyDead", false);
            animator.SetInteger("EnemyIndex", enemy.index);
            enemy.damage = storeEneDam;
            battleText.text = enemy.name + " appeared!";
            for (int i = 0; i < enemyHealthGroup.Length; i++)
            {
                enemyHealthGroup[i].GetComponent<Image>().color += new Color(0, 0, 0, 1);
            }
            StartCoroutine(BattleCoroutine());

            yield break;
        }
        else
        {
            yield return null;
        }

    }

    public void OnLevelUp()
    {
        StartCoroutine(LevelUpCoroutine());
    }

    private IEnumerator LevelUpCoroutine()
    {
        spawnEnemies = false;

        float elapsed = 0.0f;
        float duration = 5.0f;

        StopCoroutine(BattleCoroutine());
        StopCoroutine(EnemyDead());

        battleText.text = $"Level up! You are now level {Player.Instance.level}! " +
            $"hp and magic restored. " +
            $"attack and defense up! " +
            $"Enemies will also be stronger...";

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            yield return null;
        }

        while (!isAnyKeyDown)
        {
            yield return null;
        }

        battleText.text = string.Empty;
        spawnEnemies = true;
        CreateRandomEnemy(maxRange, minRange);
        yield break;
    }

}

