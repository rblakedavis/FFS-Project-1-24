using System.Collections;
using UnityEngine;
using System.Linq;

public class TooltipHandler : MonoBehaviour
{
    private string tempString;
    private string shopTooltip;
    private string lootTooltip;
    private string grindTooltip;
    private string bossTooltip;
    public float lingerTime = 1f;
    private Coroutine coroutine;

    public void OnMouseEnter()
    {
        string[] excludedTooltips = { shopTooltip, lootTooltip, grindTooltip, bossTooltip };
        if (!excludedTooltips.Contains(Data.subWindowText))        
        {
            tempString = Data.subWindowText;
        }
        switch(this.name)
        {
            case "GrindBtn":
                break;
            case "BossBtn":
                break;
            case "ShopBtn":
                shopTooltip = "Current Gold = " + Data.goldCur;
                if (Data.subWindowText != shopTooltip)
                {
                    tempString = Data.subWindowText;
                }
                Data.subWindowText = shopTooltip;
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
        Data.subWindowText = tempString;
        tempString = string.Empty;
    }

}
