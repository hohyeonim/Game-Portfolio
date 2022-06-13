using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : CloseWeaponController
{
    // Ȱ��ȭ ����
    public static bool isActivate = true;
    public static Item currentKit; // ��ġ�Ϸ��� Ŷ(���� ���̺�)

    private bool isPreview = false;

    private GameObject go_Preview; // ��ġ�� ŰƮ ������
    private Vector3 previewPos; // ��ġ�� ŰƮ ��ġ
    [SerializeField]
    private float rangeAdd; // ����� �߰� �����Ÿ�

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

    // �������̺� �̸����� ��ġ�� ��ġ
    private void InstallPreviewKit()
    {
        isPreview = true;
        go_Preview = Instantiate(currentKit.kitPreviewPrefab, transform.position, Quaternion.identity);
    }

    // �̸����� ��ġ
    private void PreviewPositionUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range + rangeAdd, layerMask))
        {
            previewPos = hitInfo.point;
            go_Preview.transform.position = previewPos;
        }
    }

    // ����
    private void Build()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (go_Preview.GetComponent<PreviewObject>().isBuildable())
            {
                theQuickSlot.DecreaseSelectedKit(); // ���� ������ ���� -1
                GameObject temp = Instantiate(currentKit.kitPrefab, previewPos, Quaternion.identity);
                temp.name = currentKit.itemName;
                Destroy(go_Preview);
                currentKit = null;
                isPreview = false;
            }
        }
    }

    // ���
    public void Cancel()
    {
        Destroy(go_Preview);
        currentKit = null;
        isPreview = false;
    }

    // ��Ŭ�� ������ ���(����, ���)
    private void TryEating()
    {
        if (Input.GetButtonDown("Fire2") && !theQuickSlot.GetIsCoolTime())
        {
            currentCloseWeapon.anim.SetTrigger("Eat");
            theQuickSlot.DecreaseSelectedItem();
        }
    }

    // �Ǽ� ���� �ڷ�ƾ(����)
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
