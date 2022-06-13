using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    // �ʿ��� ������Ʈ
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

    // ���Ե�
    private Slot[] slots; // �κ��丮 ���Ե�
    private Slot[] quickslots; // �����Ե�
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

    // �κ��丮 UI Ȱ��ȭ
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

    // �κ��丮 open
    private void OpenInventory()
    {
        // GameManager.cs ��ũ��Ʈ�� �̿��Ͽ� �ൿ���� �� ���콺 Ȱ��ȭ
        GameManager.isOpenInventory = true;

        // UI Ȱ��ȭ
        go_InventoryBase.SetActive(true);
    }

    // �κ��丮 close
    private void CloseInventory()
    {
        GameManager.isOpenInventory = false;

        go_InventoryBase.SetActive(false);
        theSlotToolTip.HideToolTip();
    }

    // ������ ȹ�� �Լ�(�κ��丮�� ������ ȹ��)
    public void AcquireItem(Item _item, int _count = 1)
    {
        PutSlot(quickslots, _item, _count); // ������ ȹ�� �� �����Ժ��� ä��� �ʹٸ� 'quikslots', �κ����� ä��� �ʹٸ� 'slots'

        if (!isNotPut) // ������ ����� ������ ȹ��
        {
            theQuickSlot.IsActivatedQuickSlot(slotNumber);
        }

        if (isNotPut) // �������� ������ �κ��丮�� ȹ��
        {
            PutSlot(slots, _item, _count);
        }
    }

    // ���� Ȯ��(�κ��丮, ������)
    private void PutSlot(Slot[] _slots, Item _item, int _count)
    {
        // ������ Ÿ���� ���, kit�� �ƴϸ� ���� �������� ��� count�� �÷��ش�.
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

        // ������ Ÿ���� ���, kit�� ��� ���� �������� ���� ��� ���� ������ �����Ѵ�.
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

    // �ش� ������ ���� ��������(���)
    public int GetItemCount(string _itemName)
    {
        int temp = SearchSlotItem(slots, _itemName);

        return temp != 0 ? temp : SearchSlotItem(quickslots, _itemName);
    }

    // ���Կ��� �ش� ������ ã��
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

    // ������ ���� üũ(kit)
    public void SetItemCount(string _itemName, int _itemCount)
    {
        if (!ItemCountAdjust(slots, _itemName, _itemCount))
        {
            ItemCountAdjust(quickslots, _itemName, _itemCount);
        }
    }

    // ������ ������ŭ �Ҹ�
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
