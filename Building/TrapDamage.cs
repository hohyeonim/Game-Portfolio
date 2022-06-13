using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float finishTime;

    private bool isHurt = false;
    private bool isActivated = false;

    // 함정 발동 코루틴
    public IEnumerator ActivatedTrapCoroutine()
    {
        isActivated = true;

        yield return new WaitForSeconds(finishTime);
        isActivated = false;
        isHurt = false;
    }

    // 해당 componet에 player가 부딪히면 데미지 적용
    private void OnTriggerEnter(Collider other)
    {
        if (isActivated)
        {
            if (!isHurt)
            {
                isHurt = true;

                if (other.transform.name == "Player")
                {
                    other.transform.GetComponent<StatusController>().DecreaseHP(damage);
                }
            }
        }
    }
}
