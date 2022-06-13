using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodOnFire : MonoBehaviour
{
    [SerializeField]
    private float time; // 익히거나 타는데 걸리는 시간
    private float currentTime;

    private bool done; // 끝났으면, 더이상 불에 있어도 계산 무시

    [SerializeField]
    private GameObject go_CookedItemPrefab; // 익혀진 혹은 탄 아이템 교체

    // 고기 아이템이 Fire tag를 가진 Componet 안에 머무를 때 일정 시간 후 새로운 아이템으로 교체
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Fire" && !done)
        {
            currentTime += Time.deltaTime;

            // 지정 시간이 되면
            if (currentTime >= time) 
            {
                done = true;
                Instantiate(go_CookedItemPrefab, transform.position, Quaternion.Euler(transform.eulerAngles)); // 다음 단계 아이템으로 생성
                Destroy(gameObject); // 기존 아이템 삭제
            }
        }
    }
}
