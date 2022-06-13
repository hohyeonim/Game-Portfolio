using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private Text txt_ItemName;
    [SerializeField]
    private Text txt_ItemDesc;
    [SerializeField]
    private Text txt_ItemHowtoUsed;

    // 인벤토리 슬롯 tooltip UI 활성화
    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.7f, -go_Base.GetComponent<RectTransform>().rect.height* 1f, 0f); // tooltip UI 위치
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.itemName; // 아이템 이름
        txt_ItemDesc.text = _item.itemDesc; // 아이템 설명

        // 타입별 사용 방법 Text출력
        if (_item.itemType == Item.ItemType.Equiment)
        {
            txt_ItemHowtoUsed.text = "우클릭 - 장착";
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            txt_ItemHowtoUsed.text = "우클릭 - 먹기";
        }
        else
        {
            txt_ItemHowtoUsed.text = "재료";
        }
            
    }

    // 인벤토리 슬롯 tooltip UI 비활성화
    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}
