using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    GameData gameData;
    [SerializeField] private TextMeshProUGUI subWindowText;
    [SerializeField] private float goldConstant;
    [SerializeField] private float minGoldMultiplier;
    [SerializeField] private float maxGoldMultiplier;
    [SerializeField] private float lootCooldown = .25f;
    private float timeSinceLastLoot = .25f;


    // Start is called before the first frame update
    private void Awake()
    {
        gameData = GameManager.Instance.gameData;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastLoot += Time.deltaTime;

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
            
        }

    }

    int GoldFormula()
    {
        float goldMultiplier = Random.Range(minGoldMultiplier + gameData.level / 10, maxGoldMultiplier + gameData.level / 10);
        int goldOutput = (int)((goldConstant + gameData.level) * goldMultiplier);

        return goldOutput;
    }
}

