using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickaxeController : CloseWeaponController
{
    // Ȱ��ȭ ����
    public static bool isActivate = false;

    

    void Update()
    {
        if (isActivate && GameManager.canPlayerAttack)
        {
            TryAttack();
        }
    }

    // ��� ���� �ڷ�ƾ(��, ��������, ���� ����, ���� ����)
    protected override IEnumerator HitCoroutine()
    {
        if (CheckObject())
        {
            if (hitInfo.transform.tag == "Rock")
            {
                hitInfo.transform.GetComponent<Rock>().Mining();
            }
            else if (hitInfo.transform.tag == "Twig")
            {
                hitInfo.transform.GetComponent<Twig>().Damage(this.transform);
            }
            else if (hitInfo.transform.tag == "WeakAnimal")
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
