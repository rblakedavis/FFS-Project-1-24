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
        Debug.Log("mouse enter");
        Debug.Log(gameData.subWindowText);
        
        string[] excludedTooltips = { shopTooltip, lootTooltip, grindTooltip, bossTooltip };




        if (!excludedTooltips.Contains(gameData.subWindowText)) 
        {
            tempString = gameData.subWindowText;
        }


        switch(this.name)
        {
            case "BossBtn":
                int playerRequiredLevel = GameManager.Instance.bossRequiredLevel;
                if (gameData.level < playerRequiredLevel)
                {
                    bossTooltip = $"You must be level {playerRequiredLevel} to fight this zone\"s boss";
                }
                else
                {
                    bossTooltip = "Fight this zone\"s boss" +
                    "No running from boss battles";
                }
                if (gameData.subWindowText != bossTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = bossTooltip;
                Debug.Log(gameData.subWindowText);


                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;
            case "GrindBtn":
                grindTooltip = "Fight monsters to level up";
                if (gameData.subWindowText != grindTooltip)
                {
                    tempString = gameData.subWindowText;
                }
                gameData.subWindowText = grindTooltip;
                Debug.Log(gameData.subWindowText);


                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;

            case "LootBtn":
                lootTooltip = "Loot to gain gold";
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

            case "ShopBtn":

                
                shopTooltip = "Current Gold = " + gameData.goldCur;
                if (gameData.subWindowText != shopTooltip)
                {
                    tempString = gameData.subWindowText;
                }                
                gameData.subWindowText = shopTooltip;


                if (coroutine != null )
                {
                    StopCoroutine(coroutine);
                }

                break;


            default:
                Debug.Log($"{this.name} not found in tooltips!");
                break;
            
        }
    }

    public void OnMouseExit()
    {
        coroutine = StartCoroutine(Linger());
    }

    private IEnumerator Linger()
    {
        yield return new WaitForSeconds( lingerTime );
        gameData.subWindowText = tempString;
        tempString = string.Empty;
    }

}
