using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "Functions", menuName = "ScriptableObjects/Functions", order = 1)]

public class Functions : ScriptableObject
{
    void WriteToDebugLog()
    {
        Debug.Log("Hey I wrote something");
    }
    void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hello");

    }
}
