using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; // 플레이어의 움직임 제어

    public static bool canPlayerAttack = true; // 플레이어 공격 제어

    public static bool isOpenInventory = false; // 인벤토리 활성화

    public static bool isOpenCraftManual = false; // 건축 메뉴창 활성화

    public static bool isComputerKit = false; // 컴퓨터 메뉴창 활성화

    public static bool isOpenArchemyTable = false; // 연금 테이블 창 활성화

    public static bool isNight = false; // 밤일 때

    public static bool isWater = false; // 물 안에 있을 때

    public static bool isPause = false; // Pause 메뉴 창 활성화

    public static bool isQuestView = false; // 퀘스트 UI 창 활성화

    public static bool isAchievementView = false; // 업적 UI 창 활성화

    public static bool isOpenShop = false; // 상점 활성화

    private WeaponManager theWeaponManager;

    private bool flag = false;

    void Update()
    {
        if (isOpenInventory || isOpenCraftManual || isOpenArchemyTable || isPause || isOpenShop || isQuestView || isAchievementView)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            WeaponSway.isActivated = false;
            canPlayerMove = false;
            canPlayerAttack = false;
        }
        else if (isComputerKit)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            WeaponSway.isActivated = false;
            canPlayerAttack = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            WeaponSway.isActivated = true;
            canPlayerMove = true;
            canPlayerAttack = true;
        }

        if (isWater)
        {
            if (!flag)
            {
                StopAllCoroutines();
                StartCoroutine(theWeaponManager.WeaponInCoroutine());
                flag = true;
            }
        }
        else
        {
            if (flag)
            {
                flag = false;
                theWeaponManager.WeaponOut();
            }
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        theWeaponManager = FindObjectOfType<WeaponManager>();
    }
}
