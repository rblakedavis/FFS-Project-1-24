using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;

public class TooltipHandler : MonoBehaviour
{
    private string tempString;
    private string shopTooltip;
    private string lootTooltip;
    private string grindTooltip;
    private string bossTooltip;
    public float lingerTime = 1f;
    private Coroutine coroutine;
    private GameData gameData;



    private void Start()
    {
        gameData = GameManager.Instance.gameData;
    }

    public void OnMouseEnter()
    {

        string[] excludedTooltips = { shopTooltip, lootTooltip, grindTooltip, bossTooltip };
        if (!excludedTooltips.Contains(gameData.subWindowText))
        {
            tempString = gameData.subWindowText;
        }
        switch (this.name)
        {
            case "GrindBtn":

                grindTooltip = "Defeat monsters to level up" +
                    "Fight until you run or " +
                    "fight until you die";
                if (gameData.subWindowText != grindTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = grindTooltip;
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                break;

            case "BossBtn":
                int requiredPlayerLevel = (1 + gameData.curZoneIndex) * 5;
                if (Player.Instance.level > requiredPlayerLevel)
                {
                    bossTooltip = $"You need to be level {requiredPlayerLevel}" +
                        "to face this zone\"s boss";
                }
                else
                {
                    bossTooltip = "Defeat the boss to move to the next zone." +
                        "No running from boss battles";
                }
                if (gameData.subWindowText != bossTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = bossTooltip;
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;

            case "ShopBtn":

                shopTooltip = "Shop for better weapons or items" +
                    "Current Gold = " + gameData.goldCur;

                if (gameData.subWindowText != shopTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = shopTooltip;
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;

            case "LootBtn":
                lootTooltip = "Loot to gain gold" +
                    $"Current Gold = {gameData.goldCur}";
                if (gameData.subWindowText != lootTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = lootTooltip;
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;
        }
    }

    public void OnMouseExit()
    {
        coroutine = StartCoroutine(Linger());
    }

    private IEnumerator Linger()
    {
        yield return new WaitForSeconds(lingerTime);
        gameData.subWindowText = tempString;
        tempString = string.Empty;
    }


}
using System;

public class Class1
{
	public Class1()
	{
	}
}
