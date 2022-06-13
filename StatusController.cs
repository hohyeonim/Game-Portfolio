using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    // ü��
    [SerializeField]
    private int hp;
    private int currentHp;

    // ���¹̳�
    [SerializeField]
    private int sp;
    private int currentSp;

    // ���¹̳� ������(�ڿ� ȸ��)
    [SerializeField]
    private int spIncreaseSpeed;

    // ���¹̳� ��ȸ�� ������
    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    // ���¹̳� ���� ����
    private bool spUsed;

    // ����
    [SerializeField]
    private int dp;
    private int currentDp;

    // �����
    [SerializeField]
    private int hungry;
    private int currentHungry;

    // ������� �پ��� �ӵ�
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // �񸶸�
    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    // �񸶸��� �پ��� �ӵ�
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // ������
    [SerializeField]
    private int satisfy;
    private int currentSatisfy;

    // ���� �̺�Ʈ(������)
    float timer = 0f;
    int waitingTime;


    // �ʿ��� �̹���
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

    // SP ���� �ð�
    private void SPRechargeTime()
    {
        // ���¹̳� ȸ�� ���� ���¹̳� ��� �� �ٽ� ��¡ �ð� ����
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

    // SP ����
    private void SPRecover()
    {
        // ������ ��¡ �ð��� �ǰ� ���� ���¹̳��� �ִ� ���¹̳� ���� ���� �� ���¹̳� ȸ��
        if (!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }
    }

    // �����
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
            //Debug.Log("����� ��ġ�� 0�� �Ǿ����ϴ�.");
        }
    }

    // �񸶸�
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
            //Debug.Log("�񸶸� ��ġ�� 0�� �Ǿ����ϴ�.");
        }
    }

    // ������ �Լ�
    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = (float)currentHp / hp;
        images_Gauge[SP].fillAmount = (float)currentSp / sp;
        images_Gauge[DP].fillAmount = (float)currentDp / dp;
        images_Gauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        images_Gauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
        images_Gauge[SATISFY].fillAmount = (float)currentSatisfy / satisfy;
    }

    // ä�� ���� �Լ�
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

    // ä�� ���� �Լ�
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
            Debug.Log("ĳ������ hp�� 0�� �Ǿ����ϴ�.");
        }
    }

    // ���� ���� �Լ�
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

    // ���� ���� �Լ�
    public void DecreaseDP(int _count)
    {
        currentDp -= _count;

        if (currentDp <= 0)
        {
            Debug.Log("ĳ������ ������ 0�� �Ǿ����ϴ�.");
        }
    }

    // ����� ���� �Լ�
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

    // ����� ���� �Լ�
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

    // �񸶸� ���� �Լ�
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

    // �񸶸� ���� �Լ�
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

    // ���¹̳� ���� �Լ�
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

    //���׹̳� ���� �Լ�
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

    // ���¹̳� ���� �� �ൿ ����
    public int GetCurrentSP()
    {
        return currentSp;
    }
}
