using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능한 최대 거리

    private bool pickupActivated = false; // 아이템 습득 가능할 시 true
    private bool dissolveActivated = false; // 고기 해체 가능할시 true
    private bool isDissolving = false; // 고기 해체 중에는 true
    private bool fireLookActivated = false; // 불을 근접해서 바라볼 시 true
    private bool lookArchemyTable = false; // 연금 테이블을 바라볼 시 true
    private bool lookComputer = false; // 컴퓨터를 바라볼 시 true
    private bool lookActivatedTrap = false; // 가동된 함정을 바라볼 시 true
    private bool lookShop = false; // 가게를 바라볼 시 true;
    private bool lookDoor = false; // 문을 바라볼 시 true;

    private RaycastHit hitInfo; // 충돌체 정보 저장

    // 아이템 레이어만 반응하도록 레이어 마스크 설정
    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트
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
    private string sound_meat; // 소리 재생


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

    // E키를 이용하여 Action 실행
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

    // 아이템 줍기
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

    // 동물이 죽었을 때 고기 해체 실행
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

    // 불 위에 아이템 떨어 뜨리기
    private void CanDropFire()
    {
        if (fireLookActivated)
        {
            if(hitInfo.transform.tag == "Fire" && hitInfo.transform.GetComponent<Fire>().GetIsFire())
            {
                Slot _selectedSlot = theQuickSlot.GetSelectedSlot();

                if (_selectedSlot.item != null)
                {
                    // 고기 아이템
                    DropAnItem(_selectedSlot);
                }
            }
        }
    }

    // 컴퓨터 전원 On
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

    // 연금 태이블 열기
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

    // 함정 재설치
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

    // 상점 열기
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

    // 문 열기
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

    // 고기 아이템만 불 위에 떨어 뜨릴 수 있음
    private void DropAnItem(Slot _selectedSlot)
    {
        switch (_selectedSlot.item.itemType)
        {
            case Item.ItemType.Used:
                if (_selectedSlot.item.itemName.Contains("고기"))
                {
                    Instantiate(_selectedSlot.item.itemPrefab, hitInfo.transform.position + Vector3.up, Quaternion.identity);
                    theQuickSlot.DecreaseSelectedItem();
                }
                break;
            case Item.ItemType.Ingredient:
                break;
        }
    }

    // 고기 해체 코루틴
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

    // 플레이어에서 레이저를 쏴서 해당 component에 tag를 확인하여 알맞는 action 실행
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

    // 아이템 획득 가능 UI 활성화
    private void ItemInfoAppear()
    {
        Reset();
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }

    // 고기 해체 가능 UI 활성화
    private void MeatInfoAppear()
    {
        if (hitInfo.transform.GetComponent<Animal>().isDead)
        {
            Reset();
            dissolveActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<Animal>().animalName + " 해체하기 " + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 불 위에 아이템 올리기 가능 UI 활성화
    private void FireInfoAppear()
    {
        Reset();
        fireLookActivated = true;

        if (hitInfo.transform.GetComponent<Fire>().GetIsFire())
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "선택된 아이템 불에 넣기" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 컴퓨터 가동 가능 UI 활성화
    private void ComputerInfoAppear()
    {
        Reset();
        lookComputer = true;

        if (!hitInfo.transform.GetComponent<ComputerKit>().isPowerOn)
        {
            actionText.gameObject.SetActive(true);
            actionText.text = "컴퓨터 가동" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 연금테이블 열기 가능 UI 활성화
    private void ArchemyInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<ArchemyTable>().GetIsOpen())
        {
            Reset();
            lookArchemyTable = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "연금 테이블 조작" + "<color=yellow>" + "(E)" + "</color>";
        }
    }
    
    // 함정 재설치 가능 UI 활성화
    private void TrapInfoAppear()
    {
        if (hitInfo.transform.GetComponent<DeadTrap>().GetIsActivated())
        {
            Reset();
            lookActivatedTrap = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "함정 재설치" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 상점 열기 가능 UI 활성화
    private void ShopInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<Shop>().GetIsOpen())
        {
            Reset();
            lookShop = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "상점 이용하기" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 문 열기 가능 UI 활성화
    private void DoorInfoAppear()
    {
        if (!hitInfo.transform.GetComponent<Door>().open || hitInfo.transform.GetComponent<Door>().open)
        {
            Reset();
            lookDoor = true;
            actionText.gameObject.SetActive(true);
            actionText.text = "문 이용하기" + "<color=yellow>" + "(E)" + "</color>";
        }
    }

    // 모든 action 활성화 false
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
