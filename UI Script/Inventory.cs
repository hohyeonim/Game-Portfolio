using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;
    [SerializeField]
    private SlotToolTip theSlotToolTip;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    [SerializeField]
    private GameObject go_QuickSlotParent;

    // 슬롯들
    private Slot[] slots; // 인벤토리 슬롯들
    private Slot[] quickslots; // 퀵슬롯들
    private bool isNotPut;
    private int slotNumber;

    public Slot[] GetSlots()
    {
        return slots;
    }

    [SerializeField]
    private Item[] items;

    public void LoadToInven(int _arrayNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].itemName == _itemName)
            {
                slots[_arrayNum].AddItem(items[i], _itemNum);
            }
        }
    }

    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>();
        quickslots = go_QuickSlotParent.GetComponentsInChildren<Slot>();
    }

    void Update()
    {
        TryOpenInventory();
    }

    // 인벤토리 UI 활성화
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }

    // 인벤토리 open
    private void OpenInventory()
    {
        // GameManager.cs 스크립트를 이용하여 행동제한 및 마우스 활성화
        GameManager.isOpenInventory = true;

        // UI 활성화
        go_InventoryBase.SetActive(true);
    }

    // 인벤토리 close
    private void CloseInventory()
    {
        GameManager.isOpenInventory = false;

        go_InventoryBase.SetActive(false);
        theSlotToolTip.HideToolTip();
    }

    // 아이템 획득 함수(인벤토리에 아이템 획득)
    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickslots, _item, _count); // 아이템 획득 시 퀵슬롯부터 채우고 싶다면 'quikslots', 인벤부터 채우고 싶다면 'slots'

        if (!isNotPut) // 퀵슬롯 빈곳에 아이템 획득
        {
            theQuickSlot.IsActivatedQuickSlot(slotNumber);
        }

        if (isNotPut) // 퀵슬롯이 꽉차면 인벤토리에 획득
        {
            PutSlot(slots, _item, _count);
        }
    }

    // 슬롯 확인(인벤토리, 퀵슬롯)
    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        // 아이템 타입이 장비, kit가 아니면 같은 아이템일 경우 count를 늘려준다.
        if (Item.ItemType.Equiment != _item.itemType && Item.ItemType.Kit != _item.itemType)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i].item != null)
                {
                    if (_slots[i].item.itemName == _item.itemName)
                    {
                        slotNumber = i;
                        _slots[i].SetSlotCount(_count);
                        isNotPut = false;
                        return;
                    }
                }
            }
        }

        // 아이템 타입이 장비, kit일 경우 동일 아이템이 들어올 경우 각각 슬롯을 차지한다.
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item == null)
            {
                _slots[i].AddItem(_item, _count);
                isNotPut = false;
                return;
            }
        }

        isNotPut = true;
    }

    // 해당 아이템 개수 가져오기(재료)
    public int GetItemCount(string _itemName)
    {
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickslots, _itemName);
    }

    // 슬롯에서 해당 아이템 찾기
    private int SearchSlotItem(Slot[] _slots, string _itemName)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                {
                    return _slots[i].itemCount;
                }
            }
        }

        return 0;
    }

    // 아이템 개수 체크(kit)
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if (!ItemCountAdjust(slots, _itemName, _itemCount))
        {
            ItemCountAdjust(quickslots, _itemName, _itemCount);
        }
    }

    // 아이템 개수만큼 소모
    private bool ItemCountAdjust(Slot[] _slots, string _itemName, int _itemCount)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].item != null)
            {
                if (_itemName == _slots[i].item.itemName)
                {
                    _slots[i].SetSlotCount(-_itemCount);
                    return true;
                }
            }
        }

        return false;
    }
}
