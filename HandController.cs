using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = true;
    public static Item currentKit; // 설치하려는 킷(연금 테이블)

    private bool isPreview = false;

    private GameObject go_Preview; // 설치할 키트 프리뷰
    private Vector3 previewPos; // 설치할 키트 위치
    [SerializeField]
    private float rangeAdd; // 건축시 추가 사정거리

    [SerializeField]
    private QuickSlotController theQuickSlot;

    private void Start()
    {
        WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
    }

    void Update()
    {
        if (isActivate && !Inventory.inventoryActivated && GameManager.canPlayerAttack)
        {
            if (currentKit == null)
            {
                if (QuickSlotController.go_HandItem == null)
                {
                    TryAttack();
                }
                else
                {
                    TryEating();
                }
            }
            else
            {
                if (!isPreview)
                {
                    InstallPreviewKit();
                }

                PreviewPositionUpdate();
                Build();
            }
        }
    }

    // 연금테이블 미리보기 위치에 설치
    private void InstallPreviewKit()
    {
        isPreview = true;
        go_Preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    // 미리보기 위치
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_Preview.transform.position = previewPos;
        }
    }

    // 건축
    private void Build()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (go_Preview.GetComponent<PreviewObject>().isBuildable())
            {
                theQuickSlot.DecreaseSelectedKit(); // 슬롯 아이템 개수 -1
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_Preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    // 취소
    public void Cancel()
    {
        Destroy(go_Preview);
        currentKit = null;
        isPreview = false;
    }

    // 우클릭 아이템 사용(물약, 고기)
    private void TryEating()
    {
        if (Input.GetButtonDown("Fire2") && !theQuickSlot.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    // 맨손 공격 코루틴(동물)
    protected override IEnumerator HitCoroutine()
    {
        if (CheckObject())
        {
            if (hitInfo.transform.tag == "WeakAnimal")
            {
                SoundManager.instance.PlaySE("Animal_Hit");
                hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentCloseWeapon.damage, transform.position);
            }
            else if (hitInfo.transform.tag == "StrongAnimal")
            {
                SoundManager.instance.PlaySE("Animal_Hit");
                hitInfo.transform.GetComponent<StrongAnimal>().Damage(currentCloseWeapon.damage, transform.position);
            }

            isSwing = false;
            Debug.Log(hitInfo.transform.name);
        }

        yield return null;
    }

    public override void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        base.CloseWeaponChange(_closeWeapon);
        isActivate = true;
    }
}
