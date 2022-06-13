using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    [SerializeField]
    private string fireName; // ���� �̸�(����, ��ں�, ȭ���)

    [SerializeField]
    private int damgae; // ���� ������

    [SerializeField]
    private float damageTime; // �������� �� ������
    private float currentDamageTime;

    [SerializeField]
    private float durationTime; // ���� ���ӽð�
    private float currentDurationTime;

    [SerializeField]
    private ParticleSystem ps_Flame; // ��ƼŬ �ý���

    //�ʿ��� ������Ʈ
    private StatusController thePlayerStatus;

    // ���� ����
    private bool isFire = true;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        currentDurationTime = durationTime;
    }


    void Update()
    {
        if (isFire)
        {
            ElapseTime();
        }
    }
    
    // ���� Ȱ��ȭ �Ǵ� �ð� ���
    private void ElapseTime()
    {
        currentDurationTime -= Time.deltaTime;

        if (currentDamageTime > 0)
        {
            currentDamageTime -= Time.deltaTime;
        }

        if (currentDurationTime <= 0)
        {
            // �Ҳ�
            Off();
        }
    }

    // ���� ����
    private void Off()
    {
        ps_Flame.Stop();
        isFire = false;
    }

    // �ش� Fire Collider �ȿ� Player�±׸� ���� Component�� �ӹ����� �������� �Դ´�
    private void OnTriggerStay(Collider other)
    {
        if (isFire && other.transform.tag == "Player")
        {
            if (currentDamageTime <= 0)
            {
                other.GetComponent<Burn>().StartBurning(); // ȭ�� ȿ��
                thePlayerStatus.DecreaseHP(damgae);
                currentDamageTime = damageTime;
            }
        }
    }

    public bool GetIsFire()
    {
        return isFire;
    }
}
