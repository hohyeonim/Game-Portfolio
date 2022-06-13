using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private Text txt_ItemName;
    [SerializeField]
    private Text txt_ItemDesc;
    [SerializeField]
    private Text txt_ItemHowtoUsed;

    // �κ��丮 ���� tooltip UI Ȱ��ȭ
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.7f, -go_Base.GetComponent<RectTransform>().rect.height* 1f, 0f); // tooltip UI ��ġ
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.itemName; // ������ �̸�
        txt_ItemDesc.text = _item.itemDesc; // ������ ����

        // Ÿ�Ժ� ��� ��� Text���
        if (_item.itemType == Item.ItemType.Equiment)
        {
            txt_ItemHowtoUsed.text = "��Ŭ�� - ����";
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            txt_ItemHowtoUsed.text = "��Ŭ�� - �Ա�";
        }
        else
        {
            txt_ItemHowtoUsed.text = "���";
        }
            
    }

    // �κ��丮 ���� tooltip UI ��Ȱ��ȭ
    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
