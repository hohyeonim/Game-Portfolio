using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool canPlayerMove = true; // �÷��̾��� ������ ����

    public static bool canPlayerAttack = true; // �÷��̾� ���� ����

    public static bool isOpenInventory = false; // �κ��丮 Ȱ��ȭ

    public static bool isOpenCraftManual = false; // ���� �޴�â Ȱ��ȭ

    public static bool isComputerKit = false; // ��ǻ�� �޴�â Ȱ��ȭ

    public static bool isOpenArchemyTable = false; // ���� ���̺� â Ȱ��ȭ

    public static bool isNight = false; // ���� ��

    public static bool isWater = false; // �� �ȿ� ���� ��

    public static bool isPause = false; // Pause �޴� â Ȱ��ȭ

    public static bool isQuestView = false; // ����Ʈ UI â Ȱ��ȭ

    public static bool isAchievementView = false; // ���� UI â Ȱ��ȭ

    public static bool isOpenShop = false; // ���� Ȱ��ȭ

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
