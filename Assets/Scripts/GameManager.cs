using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI zone;
    [SerializeField] private TextMeshProUGUI level;
    [SerializeField] private TextMeshProUGUI magic;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI subWindow;
    [SerializeField] private SceneChanger sceneChanger;

    void OnEnable()
    {
        Debug.Log("On Enable");
        Data.subWindowText = string.Empty;
        subWindow.text = Data.subWindowText;
    }

    // Update is called once per frame
    void Update()
    {
        if (subWindow.text != Data.subWindowText)
            {
                subWindow.text = Data.subWindowText;
            }
    }
}
