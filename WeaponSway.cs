using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public static bool isActivated = true;
     
    // 기존 위치
    private Vector3 originPos;

    // 현재 위치
    private Vector3 currentPos;

    // sway 한계
    [SerializeField]
    private Vector3 limitPos;

    // 정조준 sway 한계
    [SerializeField]
    private Vector3 fineSightLimitPos;
    
    // 부드러운 움직임 정도
    [SerializeField]
    private Vector3 smoothSway;

    //필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;

    void Start()
    {
        // 현재 자기 자신 위치값 대입
        originPos = this.transform.localPosition;
    }

    void Update()
    {
        if (GameManager.canPlayerMove && isActivated)
        {
            TrySway();
        }
    }

    // Sway 시도
    private void TrySway()
    {
        // 상하좌우가 움직였을 때
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
        // 임시 변수 X, Y
        float _moveX = Input.GetAxisRaw("Mouse X");
        float _moveY = Input.GetAxisRaw("Mouse Y");

        if (!theGunController.GetFineSightMode())
        {
            // sway 움직임 범위 한계치 설정
           currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.x), -limitPos.x, limitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.x), -limitPos.y, limitPos.y),
                           originPos.z);
        }
        else
        {
            // 정조준 상태일 때 움직임 범위 한계치 설정
            currentPos.Set(Mathf.Clamp(Mathf.Lerp(currentPos.x, -_moveX, smoothSway.y), -fineSightLimitPos.x, fineSightLimitPos.x),
                           Mathf.Clamp(Mathf.Lerp(currentPos.y, -_moveY, smoothSway.y), -fineSightLimitPos.y, fineSightLimitPos.y),
                           originPos.z);
        }
        
        transform.localPosition = currentPos;
    }

    // 원래 위치로 복구
    private void BackToOriginPos()
    {
        currentPos = Vector3.Lerp(currentPos, originPos, smoothSway.x);
        transform.localPosition = currentPos;
    }
}
