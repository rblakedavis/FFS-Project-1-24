using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBtns : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private float scrollSpeed;

    public void ScrollUp()
    {
        scrollbar.value= Mathf.Clamp01(scrollbar.value - scrollSpeed);
        if (scrollbar.value <=0.05f)
        {
            scrollbar.value = 0f;
        }

    }    
    public void ScrollDown()
    {
        scrollbar.value= Mathf.Clamp01(scrollbar.value + scrollSpeed);
        if (scrollbar.value >=0.95f)
        {
            scrollbar.value = 1f;
        }
    }
}
