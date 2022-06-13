using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotController : MonoBehaviour
{
    [SerializeField]
    private Slot[] quickSlots; // �����Ե�

    [SerializeField]
    private Image[] img_CoolTime; // ������ ��Ÿ��

    [SerializeField]
    private Transform tf_parent; // �������� �θ� ��ü

    [SerializeField]
    private Transform tf_ItemPos; // ������ ��ġ�� �� ��
    public static GameObject go_HandItem; // �տ� �� ������

    // ��Ÿ�� ����
    [SerializeField]
    private float coolTime;
    private float currentCoolTime;
    private bool isCoolTime;

    // ������ ���� ����
    [SerializeField]
    private float appearTime;
    private float currentAppearTime;
    private bool isAppear;

    private int selectedSlot; // ���õ� ������ (0~7) = 8��

    // �ʿ��� ������Ʈ
    [SerializeField]
    private GameObject go_SelectedImage; // ���õ� �������� �̹���
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

    // ������ ���� ���
    private void AppearCalc()
    {
        // �κ��丮�� Ȱ��ȭ �������� ������ ����
        if (Inventory.inventoryActivated)
        {
            AppearReset();
        }
        else // �κ��丮�� ��Ȱ��ȭ �Ǿ� ������ ������ ���� �ð��� �帣�� �������� �����.
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

    // ������ ��Ÿ�� ���
    private void CoolTimeCalc()
    {
        if (isCoolTime)
        {
            currentCoolTime -= Time.deltaTime; // ��Ÿ�� �ð� �帧

            // ��Ÿ���� �帣�� ����� UI ǥ��
            for (int i = 0; i < img_CoolTime.Length; i++)
            {
                img_CoolTime[i].fillAmount = currentCoolTime / coolTime;
            }
            
            // ��Ÿ���� �� �帣�� isCoolTime�� false
            if (currentCoolTime <= 0)
            {
                isCoolTime = false;
            }
        }
    }

    // ������ 1~8Ű ����
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

    // Ȱ��ȭ �� �������� �������� �ڵ����� ����
    public void IsActivatedQuickSlot(int _num)
    {
        // ������ �̹� Ȱ��ȭ �� �����̶��
        if (selectedSlot == _num)
        {
            Execute();
            return;
        }

        // �巡�� �� ������ �������̸� ������ �ѹ��� Ȱ��ȭ ���԰� ���ٸ� 
        if (DragSlot.instance != null)
        {
            if (DragSlot.instance.dragSlot.GetQuickSlotNumber() == selectedSlot)
            {
                Execute();
                return;
            }
        }
    }
        
    // ���� ��ü
    private void ChangeSlot(int _num)
    {
        SelectedSlot(_num);

        Execute();
    }

    // ���� ����
    private void SelectedSlot(int _num)
    {
        // ���õ� ����
        selectedSlot = _num;

        // ���õ� �������� ���� �׵θ� �̹��� �̵�
        go_SelectedImage.transform.position = quickSlots[selectedSlot].transform.position;
    }

    // ������ Ÿ���� Equiment, Used, Kit �� ��� ĳ���� �տ� ������ ����
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

    // �Ǽ����� ��ü(�������� ��� ������ ����)
    private void ChangeHand(Item _item = null)
    {
        StartCoroutine(theWeaponManager.ChangeWeaponCoroutine("HAND", "�Ǽ�"));

        if (_item != null)
        {         
            StartCoroutine(HandItemCoroutine(_item));
        }
    }

    // �տ� ������ ���� �ڷ�ƾ
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

    // ������ ���
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

    // ŰƮ ���
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
