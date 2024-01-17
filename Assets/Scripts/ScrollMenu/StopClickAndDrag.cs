using System.Collections;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.EventSystems;




public class StopClickAndDrag : MonoBehaviour, IDragHandler
{
    private Scrollbar scrollbar;

    private void Start()
    {
        // Get the Scrollbar component
        scrollbar = GetComponent<Scrollbar>();

        // Ensure the Scrollbar is not null
        if (scrollbar != null)
        {
            // Remove any existing Scrollbar event listeners
            scrollbar.onValueChanged.RemoveAllListeners();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ignore drag events, effectively disabling click-and-drag
    }
}

    


