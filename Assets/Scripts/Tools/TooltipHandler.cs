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
            gameData = GameData.Instance;
    }

public void OnMouseEnter()
    {
        
        string[] excludedTooltips = { shopTooltip, lootTooltip, grindTooltip, bossTooltip };




        if (!excludedTooltips.Contains(GameManager.Instance.subWindow.text)) 
        {
            tempString = GameManager.Instance.subWindow.text;
        }


        switch(this.name)
        {
            case "BossBtn":
                int playerRequiredLevel = GameManager.Instance.bossRequiredLevel;
                if (Player.Instance.level < playerRequiredLevel)
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
                    tempString = GameManager.Instance.subWindow.text;
                }
                gameData.subWindowText = bossTooltip;


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
