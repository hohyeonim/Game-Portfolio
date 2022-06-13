using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static bool isActivated = true;
     
    // ���� ��ġ
    private Vector3 originPos;

    // ���� ��ġ
    private Vector3 currentPos;

    // sway �Ѱ�
    [SerializeField]
    private Vector3 limitPos;

    // ������ sway �Ѱ�
    [SerializeField]
    private Vector3 fineSightLimitPos;
    
    // �ε巯�� ������ ����
    [SerializeField]
    private Vector3 smoothSway;

    //�ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController;

    void Start()
    {
        // ���� �ڱ� �ڽ� ��ġ�� ����
        originPos = this.transform.localPosition;
    }

    void Update()
    {
        if (GameManager.canPlayerMove && isActivated)
        {
            TrySway();
        }
    }

    // Sway �õ�
    private void TrySway()
    {
        // �����¿찡 �������� ��
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            Swaying();
        }
        else
        {
            BackToOriginPos();
        }
    }

    private void Swaying()
    {
        // �ӽ� ���� X, Y
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if (!theGunController.GetFineSightMode())
        {
            // sway ������ ���� �Ѱ�ġ ����
           currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else
        {
            // ������ ������ �� ������ ���� �Ѱ�ġ ����
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                           originPos.z);
        }
        
        transform.localPosition = currentPos;
    }

    // ���� ��ġ�� ����
    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
