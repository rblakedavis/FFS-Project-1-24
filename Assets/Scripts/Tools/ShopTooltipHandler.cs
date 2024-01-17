using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopTooltipHandler : MonoBehaviour
{
    private TextMeshProUGUI descriptionWindow;
    private TextMeshProUGUI currentGoldWindow;
    private TextMeshProUGUI itemCostWindow;
    private TextMeshProUGUI goldAfterPurchaseWindow;
    private string itemDescirption;
    private string itemCost;
    private string goldAfterPurchase;
    ItemContainer itemContainer;

    private void Awake()
    {
        itemContainer = GetComponentInChildren<ItemContainer>();
        descriptionWindow = GameObject.Find("NotifWindowText").GetComponent<TextMeshProUGUI>();
        currentGoldWindow = GameObject.Find("PlayerGoldNumber").GetComponent<TextMeshProUGUI>();
        itemCostWindow = GameObject.Find("ItemCostNumber").GetComponent<TextMeshProUGUI>();
        goldAfterPurchaseWindow = GameObject.Find("GoldAfterNumber").GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {

        if(itemContainer != null && itemContainer.item != null) 
        { 
            itemDescirption = itemContainer.item.description;
            itemCost = itemContainer.item.cost.ToString();
            goldAfterPurchase = (GameData.Instance.goldCur - itemContainer.item.cost).ToString();

        }
        else
        {
            itemDescirption = "Description not available";
        }
    }

    void Update()
    {
        currentGoldWindow.text = GameData.Instance.goldCur.ToString();
    }

    private void OnMouseEnter()
    {
        descriptionWindow.text = itemDescirption;
        goldAfterPurchaseWindow.text = goldAfterPurchase;
        itemCostWindow.text = itemCost;
    }

    private void OnMouseExit()
    {
        descriptionWindow.text = string.Empty;
        goldAfterPurchaseWindow.text = string.Empty;
        itemCostWindow.text = string.Empty;
    }
}
