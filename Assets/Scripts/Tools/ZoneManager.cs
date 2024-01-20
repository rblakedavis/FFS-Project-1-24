using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ZoneManager : MonoBehaviour
{
    [SerializeField] private List<Sprite> menuGraphic = new List<Sprite>();
    [SerializeField] private List<Sprite> subMenuGraphic = new List<Sprite>();
    [SerializeField] private List<Sprite> upArrowButton;
    [SerializeField] private List<Sprite> downArrowButton;
    [SerializeField] private List<Sprite> background = new List<Sprite>();
    private List<GameObject> subMenuObjects = new List<GameObject>();
    private List<GameObject> menuObjects = new List<GameObject>();
    private List<GameObject> backgroundObjects = new List<GameObject>();
    private int previousZoneIndex = -1;


    void Awake()
    {
        ClearZoneManagerObjectLists();
        PopulateListWithTag("MenuObjects", menuObjects);
        PopulateListWithTag("SubMenuObjects", subMenuObjects);
        PopulateListWithTag("BackgroundObjects", backgroundObjects);

        UpdateGraphics();
    }

    void Update()
    {
        if (GameData.Instance != null)
        {
            int currentZoneIndex = GameData.Instance.curZoneIndex;

            if (currentZoneIndex != previousZoneIndex)
            {
                UpdateGraphics();
                previousZoneIndex = currentZoneIndex;
            }
        }
    }

    private void ClearZoneManagerObjectLists()
    {
        subMenuObjects.Clear();
        menuObjects.Clear();
    }

    private void PopulateListWithTag(string tag, List<GameObject> objectList)
    {
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objectsWithTag)
        {
            Image imageComponent = obj.GetComponent<Image>();

            if (imageComponent != null)
            {
                objectList.Add(obj);
            }
            else
            {
                Debug.LogError($"Object with tag '{tag}' is missing Image component.");
            }
        }
    }

    private void UpdateGraphics()
    {
        if (GameData.Instance != null)
        {
            int currentZoneIndex = GameData.Instance.curZoneIndex;

            for (int i = 0; i < menuObjects.Count; i++)
            {
                menuObjects[i].GetComponent<Image>().sprite = menuGraphic[currentZoneIndex];
            }

            for (int i = 0;i < subMenuObjects.Count; i++)
            {
                subMenuObjects[i].GetComponent<Image>().sprite = subMenuGraphic[currentZoneIndex];
            }
            for (int i = 0; i<backgroundObjects.Count; i++)
            {
                backgroundObjects[i].GetComponent<Image>().sprite = background[currentZoneIndex];
            }
        }
    }

}
