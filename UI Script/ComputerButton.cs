using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private ComputerKit theComputer;
    [SerializeField]
    private int buttonNum;

    // 마우스 포인터가 해당 위치에 있으면 UI활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        theComputer.ShowToolTip(buttonNum);
    }

    // 마우스 포인터가 해당 위치에 있으면 UI비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        theComputer.HideToolTip();
    }
}
