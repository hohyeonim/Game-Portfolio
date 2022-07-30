using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour
{

    private bool isBurning = false;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float damageTime;
    private float currentDamageTime;

    [SerializeField]
    private float durationTime;
    private float currentDurationTime;

    [SerializeField]
    private GameObject flame_prefab; // 불 이펙트 prefab
    private GameObject go_tempFlame; // 그릇

    // 화상 데미지 시작
    public void StartBurning()
    {
        if (!isBurning) // 화상 입었을 때
        {
            // player 몸에 불 이펙트 생성
            go_tempFlame = Instantiate(flame_prefab, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
            go_tempFlame.transform.SetParent(transform);
        }
        isBurning = true;
        currentDurationTime = durationTime; // 일정 시간 화상 데미지 유지
    }

    void Update()
    {
        if (isBurning)
        {
            ElapseTime();
        }
    }

    // 화상 데미지 시간 계산
    private void ElapseTime()
    {
        if (isBurning)
        {
            currentDurationTime -= Time.deltaTime;

            if (currentDamageTime > 0)
            {
                currentDamageTime -= Time.deltaTime;
            }    

            if (currentDamageTime <= 0)
            {
                // 데미지 입힘
                Damage();
            }

            if (currentDurationTime <= 0)
            {
                // 화상 효과 종료
                Off();
            }
        }
    }

    // 화상 데미지
    private void Damage()
    {
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHP(damage);
    }

    // 화상 효과 종료
    private void Off()
    {
        isBurning = false;
        Destroy(go_tempFlame);
    }
}
