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
    private GameObject flame_prefab; // �� ����Ʈ prefab
    private GameObject go_tempFlame; // �׸�

    // ȭ�� ������ ����
    public void StartBurning()
    {
        if (!isBurning) // ȭ�� �Ծ��� ��
        {
            // player ���� �� ����Ʈ ����
            go_tempFlame = Instantiate(flame_prefab, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)));
            go_tempFlame.transform.SetParent(transform);
        }
        isBurning = true;
        currentDurationTime = durationTime; // ���� �ð� ȭ�� ������ ����
    }

    void Update()
    {
        if (isBurning)
        {
            ElapseTime();
        }
    }

    // ȭ�� ������ �ð� ���
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
                // ������ ����
                Damage();
            }

            if (currentDurationTime <= 0)
            {
                // ȭ�� ȿ�� ����
                Off();
            }
        }
    }

    // ȭ�� ������
    private void Damage()
    {
        currentDamageTime = damageTime;
        GetComponent<StatusController>().DecreaseHP(damage);
    }

    // ȭ�� ȿ�� ����
    private void Off()
    {
        isBurning = false;
        Destroy(go_tempFlame);
    }
}
