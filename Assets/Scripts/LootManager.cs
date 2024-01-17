using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootManager : MonoBehaviour
{
    GameData gameData;
    [SerializeField] private TextMeshProUGUI subWindowText;
    [SerializeField] private float goldConstant;
    [SerializeField] private float minGoldMultiplier;
    [SerializeField] private float maxGoldMultiplier;
    [SerializeField] private float lootCooldown = .25f;
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI goldNumber;
    [SerializeField] private TextMeshProUGUI healthNumber;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI magicNumber;
    [SerializeField] private Image magicBarFill;
    private float timeSinceLastLoot = .25f;



    // Start is called before the first frame update
    private void Awake()
    {
        gameData = GameData.Instance;
        animator.SetBool("isPlaying", true);
        goldNumber.text = gameData.goldCur.ToString();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastLoot += Time.deltaTime;
        healthNumber.text = Mathf.Floor(Player.Instance.hp).ToString();
        healthBarFill.fillAmount = Player.Instance.hp / Player.Instance.maxHP;
        magicNumber.text = Mathf.Floor(Player.Instance.magic).ToString();
        magicBarFill.fillAmount = Player.Instance.magic / Player.Instance.maxMagic;


    }

    public void LootButtonPressed()
    {
        float lootCooldown = .25f;
        if ( lootCooldown > timeSinceLastLoot)
        {
            return;
        }
        else
        {
            int addedGold = GoldFormula();
            gameData.goldCur += addedGold;
            subWindowText.text = $"you found {addedGold} gold!";
            timeSinceLastLoot = 0f;
            //play gold sound
            goldNumber.text = gameData.goldCur.ToString();


        }

    }

    public void BackButtonPressed()
    {

    }

    int GoldFormula()
    {
        float goldMultiplier = Random.Range(minGoldMultiplier + Player.Instance.level / 10, maxGoldMultiplier + Player.Instance.level / 10);
        int goldOutput = (int)((goldConstant + Player.Instance.level) * goldMultiplier);

        return goldOutput;
    }
}

