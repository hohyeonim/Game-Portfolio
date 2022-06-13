using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템 개수
    public Image itemImage; //아이템의 이미지

    [SerializeField]
    private bool isQuickSlot; // 퀵슬롯 여부 판단
    [SerializeField]
    private int quickSlotNumber; // 퀵슬롯 번호

    // 필요한 컴포넌트
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    private ItemEffectDatabase theItemEffectDatabase;
    [SerializeField]
    private RectTransform baseRect; // 인벤토리 영역
    [SerializeField]
    private RectTransform quickSlotBaseRect; // 퀵슬롯 영역
    private InputNumber theInputNumber;

    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        theInputNumber = FindObjectOfType<InputNumber>();
    }

    // 이미지 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
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

    // 아이템 개수 조정
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
        {
            ClearSlot();
        }
    }

    // 슬롯 초기화
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }

    // 우클릭 사용 시 아이템 소모
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

    // 드래그를 시작할 때 실행되는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null && Inventory.inventoryActivated)
        {
            // 해당 아이템 이동 준비
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 드래그 중일 때 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        // 해당 아이템 이동
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    // 드래그 종료 이벤트 (드래그가 끝나기만 하면 호출)
    public void OnEndDrag(PointerEventData eventData)
    {
        // 인벤토리와 퀵슬롯 영역 범위
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

    // 드래그 종료 이벤트 (다른 슬롯 위에서 드래그가 끝났을 때 호출)
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            ChangeSlot();

            if (isQuickSlot) // 인벤토리에서 퀵슬롯으로 (혹은 퀵슬롯에서 퀵슬롯으로)
            {
                // 활성화된 퀵슬롯
                theItemEffectDatabase.IsActivatedQuickSlot(quickSlotNumber);
            }
            else
            {
                // 인벤토리 -> 인벤토리 or 퀵슬롯 -> 인벤토리
                if (DragSlot.instance.dragSlot.isQuickSlot) // 퀵슬롯 -> 인벤토리
                {
                    // 활성화 된 퀵슬롯 교체작업
                    theItemEffectDatabase.IsActivatedQuickSlot(DragSlot.instance.dragSlot.quickSlotNumber);
                }
            }
        }
    }

    // 슬롯 아이템 위치 교환을 위한 임시 슬롯 함수 (A, B 아이템, C임시 슬롯 - A->B, B->C, C->A)
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

    // 마우스가 아이템이 있는 슬롯에 들어갈때 툴팁 UI 활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            theItemEffectDatabase.ShowToolTip(item, transform.position);
        }
    }

    // 아이템이 있는 슬롯에서 마우스가 나갔을때 툴팁 UI 비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
}
