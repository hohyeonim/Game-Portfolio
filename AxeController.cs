using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeController : CloseWeaponController
{
    // 활성화 여부
    public static bool isActivate = false;

    void Update()
    {
        if (isActivate && GameManager.canPlayerAttack)
        {
            TryAttack();
        }
    }

    // 도끼 공격 코루틴(나뭇잎, 나무, 약한 동물, 강한 동물)
    protected override IEnumerator HitCoroutine()
    {
        if (CheckObject())
        {
            if (hitInfo.transform.tag == "Grass")
            {
                hitInfo.transform.GetComponent<Grass>().Damage();
            }
            else if (hitInfo.transform.tag == "Tree")
            {
                hitInfo.transform.GetComponent<TreeComponent>().Chop(hitInfo.point, transform.eulerAngles.y);
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
