using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArchemyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private ArchemyTable theArchemy;
    [SerializeField]
    private int buttonNum;

    // 마우스 포인터가 해당 위치에 올라가면 UI활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        theArchemy.ShowToolTip(buttonNum);
    }

    // 마우스 포인터가 해당 위치를 빠져 나오면 UI비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        theArchemy.HideToolTip();
    }
}
