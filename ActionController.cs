using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�

    private bool pickupActivated = false; // ������ ���� ������ �� true
    private bool dissolveActivated = false; // ��� ��ü �����ҽ� true
    private bool isDissolving = false; // ��� ��ü �߿��� true
    private bool fireLookActivated = false; // ���� �����ؼ� �ٶ� �� true
    private bool lookArchemyTable = false; // ���� ���̺��� �ٶ� �� true
    private bool lookComputer = false; // ��ǻ�͸� �ٶ� �� true
    private bool lookActivatedTrap = false; // ������ ������ �ٶ� �� true
    private bool lookShop = false; // ���Ը� �ٶ� �� true;
    private bool lookDoor = false; // ���� �ٶ� �� true;

    private RaycastHit hitInfo; // �浹ü ���� ����

    // ������ ���̾ �����ϵ��� ���̾� ����ũ ����
    [SerializeField]
    private LayerMask layerMask;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private WeaponManager theWeaponManager;
    [SerializeField]
    private Transform tf_MeatDissolveTool;
    [SerializeField]
    private QuickSlotController theQuickSlot;
    private ComputerKit theComputer;
    private ItemPickUp itemPickUp;
    private Door door;

    [SerializeField]
    private string sound_meat; // �Ҹ� ���


    void Start()
    {
        theComputer = FindObjectOfType<ComputerKit>();
        itemPickUp = FindObjectOfType<ItemPickUp>();
        door = FindObjectOfType<Door>();
    }

    void Update()
    {
        TryAction();
        CheckAction();
    }

    // EŰ�� �̿��Ͽ� Action ����
    public void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckAction();
            CanPickUp();
            CanMeat();
            CanDropFire();
            CanComputerPowerOn();
            CanArchemyTableOpen();
            CanReInstallTrap();
            CanShopOpen();
            CanDoorOpen();
        }

    }

    // ������ �ݱ�
    public void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    // ������ �׾��� �� ��� ��ü ����
    private void CanMeat()
    {
        if (dissolveActivated)
        {
            if ((hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal") && hitInfo.transform.GetComponent<Animal>().isDead && !isDissolving)
            {
                isDissolving = true;
                InfoDisappear();

                StartCoroutine(MeatCoroutine());
            }
            
        }
    }

    // �� ���� ������ ���� �߸���
    private void CanDropFire()
    {
        if (fireLookActivated)
        {
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                Slot _selectedSlot = theQuickSlot.GetSelectedSlot();

                if (_selectedSlot.item != null)
                {
                    // ��� ������
                    DropAnItem(_selectedSlot);
                }
            }
        }
    }

    // ��ǻ�� ���� On
    private void CanComputerPowerOn()
    {
        if (lookComputer)
        {
            if (hitInfo.transform != null)
            {
                if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
                {
                    hitInfo.transform.GetComponent<ComputerKit>().PowerOn();
                    InfoDisappear();
                }
            }
        }
    }

    // ���� ���̺� ����
    private void CanArchemyTableOpen()
    {
        if (lookArchemyTable)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<ArchemyTable>().Window();
                InfoDisappear();
            }
        }
    }

    // ���� �缳ġ
    private void CanReInstallTrap()
    {
        if (lookActivatedTrap)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<DeadTrap>().ReInstall();
                InfoDisappear();
            }
        }
    }

    // ���� ����
    private void CanShopOpen()
    {
        if (lookShop)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<Shop>().Window();
                InfoDisappear();
            }
        }
    }

    // �� ����
    private void CanDoorOpen()
    {
        if (lookDoor)
        {
            if (hitInfo.transform != null)
            {
                hitInfo.transform.GetComponent<Door>().ChangeDoorState();
                InfoDisappear();
            }
        }
    }

    // ��� �����۸� �� ���� ���� �߸� �� ����
    private void DropAnItem(Slot _selectedSlot)
    {
        switch (_selectedSlot.item.itemType)
        {
            case Item.ItemType.Used:
                if (_selectedSlot.item.itemName.Contains("���"))
                {
                    Instantiate(_selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }

    // ��� ��ü �ڷ�ƾ
    IEnumerator MeatCoroutine()
    {
        WeaponManager.isChangeWeapon = true;
        WeaponSway.isActivated = false;
        WeaponManager.currentWeaponAnim.SetTrigger("Weapon_Out");
        PlayerController.isActivated = false;
        yield return new WaitForSeconds(0.2f);

        WeaponManager.currentWeapon.gameObject.SetActive(false);
        tf_MeatDissolveTool.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.2f);
        SoundManager.instance.PlaySE(sound_meat);
        yield return new WaitForSeconds(1.8f);

        theInventory.AcquireItem(hitInfo.transform.GetComponent<Animal>().GetItem(), hitInfo.transform.GetComponent<Animal>().itemNumber);

        WeaponManager.currentWeapon.gameObject.SetActive(true);
        tf_MeatDissolveTool.gameObject.SetActive(false);

        PlayerController.isActivated = true;
        WeaponSway.isActivated = true;
        WeaponManager.isChangeWeapon = false;
        isDissolving = false;
    }

    // �÷��̾�� �������� ���� �ش� component�� tag�� Ȯ���Ͽ� �˸´� action ����
    private void CheckAction()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            else if (hitInfo.transform.tag == "WeakAnimal" || hitInfo.transform.tag == "StrongAnimal")
            {
                MeatInfoAppear();
            }
            else if (hitInfo.transform.tag == "Fire")
            {
                FireInfoAppear();
            }
            else if (hitInfo.transform.tag == "Computer")
            {
                ComputerInfoAppear();
            }
            else if (hitInfo.transform.tag == "ArchemyTable")
            {
                ArchemyInfoAppear();
            }
            else if (hitInfo.transform.tag == "Trap")
            {
                TrapInfoAppear();
            }
            else if (hitInfo.transform.tag == "Shop")
            {
                ShopInfoAppear();
            }
            else if (hitInfo.transform.tag == "Door")
            {
                DoorInfoAppear();
            }
            else
            {
                InfoDisappear();
            }
        }
        else
        {
            InfoDisappear();
        }
    }

    private void Reset()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
    }

    // ������ ȹ�� ���� UI Ȱ��ȭ
    private void ItemInfoAppear()
    {
        Reset();
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " ȹ�� " + "<color=yellow>" + "(E)" + "</color>";
    }

    // ��� ��ü ���� UI Ȱ��ȭ
    private void MeatInfoAppear()
    {
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            Reset();
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().animalName + " ��ü�ϱ� " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // �� ���� ������ �ø��� ���� UI Ȱ��ȭ
    private void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;

        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "���õ� ������ �ҿ� �ֱ�" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // ��ǻ�� ���� ���� UI Ȱ��ȭ
    private void ComputerInfoAppear()
    {
        Reset();
        lookComputer = true;

        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "��ǻ�� ����" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // �������̺� ���� ���� UI Ȱ��ȭ
    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            Reset();
            lookArchemyTable = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "���� ���̺� ����" + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    
    // ���� �缳ġ ���� UI Ȱ��ȭ
    private void TrapInfoAppear()
    {
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            Reset();
            lookActivatedTrap = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "���� �缳ġ" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // ���� ���� ���� UI Ȱ��ȭ
    private void ShopInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<Shop>().GetIsOpen())
        {
            Reset();
            lookShop = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "���� �̿��ϱ�" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // �� ���� ���� UI Ȱ��ȭ
    private void DoorInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<Door>().open || hitInfo.transform.GetComponent<Door>().open)
        {
            Reset();
            lookDoor = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "�� �̿��ϱ�" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // ��� action Ȱ��ȭ false
    private void InfoDisappear()
    {
        pickupActivated = false;
        dissolveActivated = false;
        fireLookActivated = false;
        lookComputer = false;
        lookArchemyTable = false;
        lookActivatedTrap = false;
        lookShop = false;
        lookDoor = false;
        actionText.gameObject.SetActive(false);
    }

}
