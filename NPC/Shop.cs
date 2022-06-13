using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    private bool isOpen = false;

    public GameObject base_Ui;
    public Animator anim;

    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private Gun theGun;

    [SerializeField]
    private Item[] items;

    void Update()
    {
        if (isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseWindow();
            }
        }
    }

    // isopen으로 상점을 열었는지 닫았는지 체크
    public void Window()
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            OpenWindow();
        }
        else
        {
            CloseWindow();
        }
    }

    // 상점을 열었을 때 UI 활성화(GameManger를 이용하여 이동 제한, 카메라 제한, 마우스 활성화)
    private void OpenWindow()
    {
        isOpen = true;
        GameManager.isOpenShop = true;
        base_Ui.SetActive(true);
    }

    // 상점 닫기
    private void CloseWindow()
    {
        isOpen = false;
        GameManager.isOpenShop = false;
        base_Ui.SetActive(false);
        anim.SetTrigger("Hello");
    }

    public bool GetIsOpen()
    {
        return isOpen;
    }

    // 버튼에 클릭에 따른 아이템 획득 함수
    // 도끼
    public void ClickButtonAxe()
    {
        theInventory.AcquireItem(items[0], 1);
    }
    // 곡괭이
    public void ClickButtonPickAxe()
    {
        theInventory.AcquireItem(items[1], 1);
    }
    // 총
    public void ClickButtonGun()
    {
        theInventory.AcquireItem(items[2], 1);
    }
    // 탄약
    public void ClickButtonBullet()
    {
        theGun.carryBulletCount += 30;
    }
}
