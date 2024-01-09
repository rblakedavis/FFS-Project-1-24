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
        switch(this.name)
        {
            case "GrindBtn":

                break;

            case "BossBtn":

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

            case "LootBtn":

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
