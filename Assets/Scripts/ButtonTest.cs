using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDebug : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Button was clicked!");
    }
}