using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBtns : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float scrollSpeed = 0.3360216f;

    public void ScrollUp()
    {
        scrollbar.value= Mathf.Clamp01(scrollbar.value - scrollSpeed);
    }    
    public void ScrollDown()
    {
        scrollbar.value= Mathf.Clamp01(scrollbar.value + scrollSpeed);
    }
}
