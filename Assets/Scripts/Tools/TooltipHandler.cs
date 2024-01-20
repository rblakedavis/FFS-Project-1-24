using System.Collections;
using UnityEngine;
using System.Linq;
using TMPro;

public class TooltipHandler : MonoBehaviour
{
    private string tempString;
    
    private string bossTooltip;
    private string bossTooltip1;
    private string bossTooltip2;
    private string grindTooltip;
    private string lootTooltip;
    private string shopTooltip;

    public float lingerTime = 1f;
    private Coroutine coroutine;
    private GameData gameData;




    private void Start()
    {
        gameData = GameData.Instance;

        bossTooltip1 = $"You must be level {GameManager.Instance.bossRequiredLevel} to fight this boss";
        bossTooltip2 = "No running from bosses";
        grindTooltip = $"exp: {Player.Instance.experience} \n level up at {Player.Instance.expNextLevel}";
        lootTooltip = "Loot to gain gold";
        shopTooltip = $"Gold: {GameData.Instance.goldCur} \n\n Gain levels = unlock more items." ;
    }

public void OnMouseEnter()
    {
        string[] excludedTooltips = { shopTooltip, lootTooltip, grindTooltip, bossTooltip1, bossTooltip2 };

        switch (this.name)
        {
            case "BossBtn":
                int playerRequiredLevel = GameManager.Instance.bossRequiredLevel;
                if (Player.Instance.level < playerRequiredLevel)
                {
                    bossTooltip = bossTooltip1;
                }
                else
                {
                    bossTooltip = bossTooltip2;
                }
                if (!excludedTooltips.Contains(GameManager.Instance.subWindow.text))
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
                if (!excludedTooltips.Contains(GameManager.Instance.subWindow.text))
                {
                    tempString = GameManager.Instance.subWindow.text;
                }
                gameData.subWindowText = grindTooltip;


                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;

            case "LootBtn":
                if (!excludedTooltips.Contains(GameManager.Instance.subWindow.text))
                {
                    tempString = GameManager.Instance.subWindow.text;
                }
                gameData.subWindowText = lootTooltip;


                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }

                break;

            case "ShopBtn":

                
                if (!excludedTooltips.Contains(GameManager.Instance.subWindow.text))
                {
                    tempString = GameManager.Instance.subWindow.text;
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
    public void OnMouseDown()
    {
        gameData.subWindowText = tempString;
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
        StopCoroutine(coroutine);
    }

}
