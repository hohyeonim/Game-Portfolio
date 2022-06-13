using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // ȹ���� ������
    public int itemCount; // ȹ���� ������ ����
    public Image itemImage; //�������� �̹���

    [SerializeField]
    private bool isQuickSlot; // ������ ���� �Ǵ�
    [SerializeField]
    private int quickSlotNumber; // ������ ��ȣ

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    [SerializeField]
    private RectTransform baseRect; // �κ��丮 ����
    [SerializeField]
    private RectTransform quickSlotBaseRect; // ������ ����
    private InputNumber theInputNumber;

    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        theInputNumber = FindObjectOfType<InputNumber>();
    }

    // �̹��� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // ������ ȹ��
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (item.itemType != Item.ItemType.Equiment)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }

        SetColor(1);
    }

    public int GetQuickSlotNumber()
    {
        return quickSlotNumber;
    }

    // ������ ���� ����
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }
    }

    // ���� �ʱ�ȭ
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    // ��Ŭ�� ��� �� ������ �Ҹ�
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                theItemEffectDatabase.UseItem(item);

                if (item.itemType == Item.ItemType.Used)
                {
                    SetSlotCount(-1);
                }
            }
        }
    }

    // �巡�׸� ������ �� ����Ǵ� �̺�Ʈ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && Inventory.inventoryActivated)
        {
            // �ش� ������ �̵� �غ�
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // �巡�� ���� �� �̺�Ʈ
    public void OnDrag(PointerEventData eventData)
    {
        // �ش� ������ �̵�
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // �巡�� ���� �̺�Ʈ (�巡�װ� �����⸸ �ϸ� ȣ��)
    public void OnEndDrag(PointerEventData eventData)
    {
        // �κ��丮�� ������ ���� ����
        if (!((DragSlot.instance.transform.localPosition.x > baseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < baseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > baseRect.rect.yMin && DragSlot.instance.transform.localPosition.y < baseRect.rect.yMax)
            ||
            (DragSlot.instance.transform.localPosition.x > quickSlotBaseRect.rect.xMin && DragSlot.instance.transform.localPosition.x < quickSlotBaseRect.rect.xMax
            && DragSlot.instance.transform.localPosition.y > quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMax - baseRect.localPosition.y && DragSlot.instance.transform.localPosition.y < quickSlotBaseRect.transform.localPosition.y - quickSlotBaseRect.rect.yMin - baseRect.localPosition.y)))
        {
            if (DragSlot.instance.dragSlot != null)
            {
                theInputNumber.Call();
            }
        }
        else 
        {
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;
        }
    }

    // �巡�� ���� �̺�Ʈ (�ٸ� ���� ������ �巡�װ� ������ �� ȣ��)
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            if (isQuickSlot) // �κ��丮���� ���������� (Ȥ�� �����Կ��� ����������)
            {
                // Ȱ��ȭ�� ������
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            }
            else
            {
                // �κ��丮 -> �κ��丮 or ������ -> �κ��丮
                if (DragSlot.instance.dragSlot.isQuickSlot) // ������ -> �κ��丮
                {
                    // Ȱ��ȭ �� ������ ��ü�۾�
                    theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
                }
            }
        }
    }

    // ���� ������ ��ġ ��ȯ�� ���� �ӽ� ���� �Լ� (A, B ������, C�ӽ� ���� - A->B, B->C, C->A)
    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
        {
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }

    // ���콺�� �������� �ִ� ���Կ� ���� ���� UI Ȱ��ȭ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            theItemEffectDatabase.ShowToolTip(item, transform.position);
        }
    }

    // �������� �ִ� ���Կ��� ���콺�� �������� ���� UI ��Ȱ��ȭ
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
}
