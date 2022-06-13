using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // 체력
    [SerializeField]
    private int hp;
    private int currentHp;

    // 스태미너
    [SerializeField]
    private int sp;
    private int currentSp;

    // 스태미너 증가량(자연 회복)
    [SerializeField]
    private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부
    private bool spUsed;

    // 방어력
    [SerializeField]
    private int dp;
    private int currentDp;

    // 배고픔
    [SerializeField]
    private int hungry;
    private int currentHungry;

    // 배고픔이 줄어드는 속도
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 목마름
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // 목마름이 줄어드는 속도
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // 지연 이벤트(데미지)
    float timer = 0f;
    int waitingTime;


    // 필요한 이미지
    [SerializeField]
    private Image[] images_Gauge;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    void Start()
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }

    void Update()
    {
        SPRechargeTime();
        SPRecover();
        Hungry();
        Thirsty();
        GaugeUpdate();
    }

    // SP 충전 시간
    private void SPRechargeTime()
    {
        // 스태미나 회복 전에 스태미나 사용 시 다시 차징 시간 리셋
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
            {
                currentSpRechargeTime++;
            }
            else
            {
                spUsed = false;
            }
        }
    }

    // SP 복구
    private void SPRecover()
    {
        // 지정된 차징 시간이 되고 현재 스태미나가 최대 스태미나 보다 작을 시 스태미나 회복
        if (!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }
    }

    // 배고픔
    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
            {
                currentHungryDecreaseTime++;
            }
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
            waitingTime = 5;

            if (timer > waitingTime)
            {
                DecreaseHP(1);
                timer = 0;
            }
            //Debug.Log("배고픔 수치가 0이 되었습니다.");
        }
    }

    // 목마름
    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
            {
                currentThirstyDecreaseTime++;
            }
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
            waitingTime = 10;

            if (timer > waitingTime)
            {
                DecreaseHP(1);
                timer = 0;
            }
            //Debug.Log("목마름 수치가 0이 되었습니다.");
        }
    }

    // 게이지 함수
    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    // 채력 증가 함수
    public void IncreaseHP(int _count)
    {
        if (currentHp + _count < hp)
        {
            currentHp += _count;
        }
        else
        {
            currentHp = hp;
        }
    }

    // 채력 감소 함수
    public void DecreaseHP(int _count)
    {
        if (currentDp > 0)
        {
            DecreaseDP(_count);
            return;
        }

        currentHp -= _count;

        if (currentHp <= 0)
        {
            Debug.Log("캐릭터의 hp가 0이 되었습니다.");
        }
    }

    // 방어력 증가 함수
    public void IncreaseDP(int _count)
    {
        if (currentDp + _count < dp)
        {
            currentDp += _count;
        }
        else
        {
            currentDp = dp;
        }
    }

    // 방어력 감소 함수
    public void DecreaseDP(int _count)
    {
        currentDp -= _count;

        if (currentDp <= 0)
        {
            Debug.Log("캐릭터의 방어력이 0이 되었습니다.");
        }
    }

    // 배고픔 증가 함수
    public void IncreaseHungry(int _count)
    {
        if (currentHungry + _count < hungry)
        {
            currentHungry += _count;
        }
        else
        {
            currentHungry = hungry;
        }
    }

    // 배고픔 감소 함수
    public void DecreaseHungry(int _count)
    {
        if (currentHungry - _count < 0)
        {
            currentHungry = 0;
        }    
        else
        {
            currentHungry -= _count;
        }
    }

    // 목마름 증가 함수
    public void IncreaseThirsty(int _count)
    {
        if (currentThirsty + _count < thirsty)
        {
            currentThirsty += _count;
        }
        else
        {
            currentThirsty = thirsty;
        }
    }

    // 목마름 감소 함수
    public void DecreaseThirsty(int _count)
    {
        if (currentThirsty - _count < 0)
        {
            currentThirsty = 0;
        }
        else
        {
            currentThirsty -= _count;
        }
    }

    // 스태미너 증가 함수
    public void IncreaseSP(int _count)
    {
        if (currentSp + _count < sp)
        {
            currentSp += _count;
        }
        else
        {
            currentSp = sp;
        }
    }

    //스테미너 감소 함수
    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
        {
            currentSp -= _count;
        }
        else
        {
            currentSp = 0;
        }
    }

    // 스태미너 없을 때 행동 제한
    public int GetCurrentSP()
    {
        return currentSp;
    }
}
