using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField]
    private Slot[] quickSlots; // 퀵슬롯들

    [SerializeField]
    private Image[] img_CoolTime; // 퀵슬롯 쿨타임

    [SerializeField]
    private Transform tf_parent; // 퀵슬롯의 부모 객체

    [SerializeField]
    private Transform tf_ItemPos; // 아이템 위치할 손 끝
    public static GameObject go_HandItem; // 손에 든 아이템

    // 쿨타임 내용
    [SerializeField]
    private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    // 퀵슬롯 등장 내용
    [SerializeField]
    private float appearTime;
    private float currentAppearTime;
    private bool isAppear;

    private int selectedSlot; // 선택된 퀵슬롯 (0~7) = 8개

    // 필요한 컴포넌트
    [SerializeField]
    private GameObject go_SelectedImage; // 선택된 퀵슬롯의 이미지
    [SerializeField]
    private WeaponManager theWeaponManager;
    private Animator anim;
    private ItemEffectDatabase theItemEffectDatabase;

    void Start()
    {
        quickSlots = tf_parent.GetComponentsInChildren<Slot>();
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        anim = GetComponent<Animator>();
        selectedSlot = 0;
    }

    void Update()
    {
        TryInputNumber();
        CoolTimeCalc();
        AppearCalc();
    }

    private void AppearReset()
    {
        currentAppearTime = appearTime;
        isAppear = true;
        anim.SetBool("Appear", isAppear);
    }

    // 퀵슬롯 숨김 계산
    private void AppearCalc()
    {
        // 인벤토리가 활성화 되있으면 퀵슬롯 고정
        if (Inventory.inventoryActivated)
        {
            AppearReset();
        }
        else // 인벤토리가 비활성화 되어 있으면 지정된 숨김 시간이 흐르고 퀵슬롯을 숨긴다.
        {
            if (isAppear)
            {
                currentAppearTime -= Time.deltaTime;
                if (currentAppearTime <= 0)
                {
                    isAppear = false;
                    anim.SetBool("Appear", isAppear);
                }
            }
        }
    }

    private void CoolTimeReset()
    {
        currentCoolTime = coolTime;
        isCoolTime = true;
    }

    // 퀵슬롯 쿨타임 계산
    private void CoolTimeCalc()
    {
        if (isCoolTime)
        {
            currentCoolTime -= Time.deltaTime; // 쿨타임 시간 흐름

            // 쿨타임이 흐르는 모습을 UI 표현
            for (int i = 0; i < img_CoolTime.Length; i++)
            {
                img_CoolTime[i].fillAmount = currentCoolTime / coolTime;
            }
            
            // 쿨타임이 다 흐르면 isCoolTime은 false
            if (currentCoolTime <= 0)
            {
                isCoolTime = false;
            }
        }
    }

    // 퀵슬롯 1~8키 선택
    private void TryInputNumber()
    {
        if (!isCoolTime)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                ChangeSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ChangeSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                ChangeSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                ChangeSlot(3);
            else if (Input.GetKeyDown(KeyCode.Alpha5))
                ChangeSlot(4);
            else if (Input.GetKeyDown(KeyCode.Alpha6))
                ChangeSlot(5);
            else if (Input.GetKeyDown(KeyCode.Alpha7))
                ChangeSlot(6);
            else if (Input.GetKeyDown(KeyCode.Alpha8))
                ChangeSlot(7);
        }
    }

    // 활성화 된 퀵슬롯의 아이템을 자동으로 장착
    public void IsActivatedQuickSlot(int _num)
    {
        // 슬롯이 이미 활성화 된 슬롯이라면
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }

        // 드래그 된 슬롯이 퀵슬롯이며 퀵슬롯 넘버가 활성화 슬롯과 같다면 
        if (DragSlot.instance != null)
        {
            if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
            {
                Execute();
                return;
            }
        }
    }
        
    // 슬롯 교체
    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);

        Execute();
    }

    // 슬롯 선택
    private void SelectedSlot(int _num)
    {
        // 선택된 슬롯
        selectedSlot = _num;

        // 선택된 슬롯으로 선택 테두리 이미지 이동
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    // 아이템 타입이 Equiment, Used, Kit 인 경우 캐릭터 손에 아이템 장착
    private void Execute()
    {
        CoolTimeReset();
        AppearReset();

        if (quickSlots[selectedSlot].item != null)
        {
            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Equiment)
            {
                StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(quickSlots[selectedSlot].item.weaponType, quickSlots[selectedSlot].item.itemName));
            }
            else if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Used || quickSlots[selectedSlot].item.itemType == Item.ItemType.Kit)
            {
                ChangeHand(quickSlots[selectedSlot].item);
            }
            else
            {
                ChangeHand();
            }
        }
        else
        {
            ChangeHand();
        }
    }

    // 맨손으로 교체(아이템일 경우 아이템 장착)
    private void ChangeHand(Item _item = null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "맨손"));

        if (_item != null)
        {         
            StartCoroutine(HandItemCoroutine(_item));
        }
    }

    // 손에 아이템 장착 코루틴
    IEnumerator HandItemCoroutine(Item _item)
    {
        HandController.isActivate = false;

        yield return new WaitUntil(() => HandController.isActivate);

        if (_item.itemType == Item.ItemType.Kit)
        {
            HandController.currentKit = _item;
        }

        go_HandItem = Instantiate(quickSlots[selectedSlot].item.itemPrefab, tf_ItemPos.position, tf_ItemPos.rotation);
        go_HandItem.GetComponent<Rigidbody>().isKinematic = true;
        go_HandItem.GetComponent<BoxCollider>().enabled = false;
        go_HandItem.tag = "Untagged";
        go_HandItem.layer = 9; // Weapon
        go_HandItem.transform.SetParent(tf_ItemPos);
    }

    // 아이템 사용
    public void DecreaseSelectedItem()
    {
        CoolTimeReset();
        AppearReset();

        if (quickSlots[selectedSlot].item != null)
        {
            theItemEffectDatabase.UseItem(quickSlots[selectedSlot].item);

            if (quickSlots[selectedSlot].item.itemType == Item.ItemType.Used)
            {
                quickSlots[selectedSlot].SetSlotCount(-1);
            }
        }

        if (quickSlots[selectedSlot].itemCount <= 0)
        {
            Destroy(go_HandItem);
        }
    }

    // 키트 사용
    public void DecreaseSelectedKit()
    {
        CoolTimeReset();
        AppearReset();

        quickSlots[selectedSlot].SetSlotCount(-1);

        if (quickSlots[selectedSlot].itemCount <= 0)
            Destroy(go_HandItem);
    }

    public bool GetIsCoolTime()
    {
        return isCoolTime;
    }

    public Slot GetSelectedSlot()
    {
        return quickSlots[selectedSlot];
    }
}
