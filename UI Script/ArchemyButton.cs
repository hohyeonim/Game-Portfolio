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

    // ���콺 �����Ͱ� �ش� ��ġ�� �ö󰡸� UIȰ��ȭ
    public void OnPointerEnter(PointerEventData eventData)
    {
        theArchemy.ShowToolTip(buttonNum);
    }

    // ���콺 �����Ͱ� �ش� ��ġ�� ���� ������ UI��Ȱ��ȭ
    public void OnPointerExit(PointerEventData eventData)
    {
        theArchemy.HideToolTip();
    }
}
