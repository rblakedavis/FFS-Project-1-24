using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopTooltipHandler : MonoBehaviour
{
    private TextMeshProUGUI descriptionWindow;
    private TextMeshProUGUI currentGold;
    private TextMeshProUGUI itemCost;
    private TextMeshProUGUI goldAfterPurchase;
    private string itemDescirption;
    ItemContainer itemContainer;

    private void Awake()
    {
        itemContainer = GetComponentInChildren<ItemContainer>();
        descriptionWindow = GameObject.Find("NotifWindow").GetComponentInChildren<TextMeshProUGUI>();
        currentGold = GameObject.Find("PlayerGoldText").GetComponentInChildren<TextMeshProUGUI>();
        itemCost = GameObject.Find("ItemCostText").GetComponentInChildren<TextMeshProUGUI>();
        goldAfterPurchase = GameObject.Find("GoldAfterText").GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        if(itemContainer != null && itemContainer.item != null) 
        { 
            itemDescirption = itemContainer.item.description;
        }
        else
        {
            itemDescirption = "Description not available";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        
    }
}
