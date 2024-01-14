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

    private void Awake()
    {
        
    }

    private void Start()
    {
        InstantiateButtons();
    }

    private void Update()
    {
        Debug.Log("Update");
    }

    private void InstantiateButtons()
    {
        foreach (Item item in items)
        {
            Debug.Log("item name is " +  item.itemName);
            if (Player.Instance.level >= item.minLevelAvailable &&
                GameData.Instance.curZoneIndex >= item.minZoneAvailable)
            {
                GameObject button = Instantiate(buttonPrefab, content);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = item.itemName;
                }
            }
        }
    }
}
