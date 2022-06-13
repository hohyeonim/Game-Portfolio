using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    // 크로스헤어 상태에 따른 총의 정확도
    private float gunAccuracy;

    // 크로스헤어 활성화/비활성화
    [SerializeField]
    private GameObject go_CrosshairHUD;

    // 필요한 컴포넌트
    [SerializeField]
    private GunController theGunController;

    // 걷기 애니메이션
    public void WalkingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            WeaponManager.currentWeaponAnim.SetBool("Walk", _flag);
            animator.SetBool("Walking", _flag);
        }
    }

    // 달리기 애니메이션
    public void RunningAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            WeaponManager.currentWeaponAnim.SetBool("Run", _flag);
            animator.SetBool("Running", _flag);
        }
    }

    // 점프 애니메이션
    public void JumpingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            animator.SetBool("Running", _flag);
        }
    }

    // 앉기 애니메이션
    public void CrouchingAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            animator.SetBool("Crouching", _flag);
        }
    }

    // 정조준 애니메이션(총)
    public void FineSightAnimation(bool _flag)
    {
        if (!GameManager.isWater)
        {
            animator.SetBool("FineSight", _flag);
        }
    }

    // 에임 애니메이션(행동에 따른 에임 상태 변경)
    public void FireAnimation()
    {
        if (!GameManager.isWater)
        {
            if (animator.GetBool("Walking"))
            {
                animator.SetTrigger("Walk_Fire");
            }
            else if (animator.GetBool("Crouching"))
            {
                animator.SetTrigger("Crouch_Fire");
            }
            else
            {
                animator.SetTrigger("Idle_Fire");
            }
        }
    }
            
    // 총 정확도(행동에 따른 정확도)
    public float GetAccuracy()
    {
        if (animator.GetBool("Walking"))
        {
            gunAccuracy = 0.06f;
        }
        else if (animator.GetBool("Crouching"))
        {
            gunAccuracy = 0.015f;
        }
        else if (theGunController.GetFineSightMode())
        {
            gunAccuracy = 0.001f;
        }
        else
        {
            gunAccuracy = 0.035f;
        }

        return gunAccuracy;
    }
}
