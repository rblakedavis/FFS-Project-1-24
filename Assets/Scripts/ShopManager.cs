using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] List<Item> items = new List<Item>();
    [SerializeField] Transform content;
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] ShopTooltipHandler tooltipHandler;

    private void Awake()
    {
        InstantiateButtons();

    }

    private void Start()
    {
    }

    private void Update()
    {
        Debug.Log("Update");
    }

    private void InstantiateButtons()
    {
        foreach (Item item in items)
        {
            

            
            if (!Player.Instance.HasItem(item) && Player.Instance.level >= item.minLevelAvailable &&
                GameData.Instance.curZoneIndex >= item.minZoneAvailable)
            {
                GameObject button = Instantiate(buttonPrefab, content);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = item.itemName;
                }
                button.tag = "SubMenuObjects";

                GameObject itemObject = new GameObject(item.itemName);
                itemObject.transform.SetParent(button.transform);
                ItemContainer itemContainer = itemObject.AddComponent<ItemContainer>();
                itemContainer.item = item;

                button.GetComponent<Button>().onClick.AddListener(() => HandleButtonClick(item, button));
                ShopTooltipHandler tooltipHandler = button.AddComponent<ShopTooltipHandler>();

            }
        }
    }
    private void HandleButtonClick(Item item, GameObject button)
    {
        if (!Player.Instance.HasItem(item))
        {
            if(item.typeOfItem != "Consumable" && GameData.Instance.goldCur >= item.cost)
            {
                item.itemPurchased(button);
                Player.Instance.AttachItem(item);
            }
            else if (item.typeOfItem == "Consumable" && GameData.Instance.goldCur >= item.cost)
            {
                item.itemPurchased(button);
            }
        }
    }
}
