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

    // isopen���� ������ �������� �ݾҴ��� üũ
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

    // ������ ������ �� UI Ȱ��ȭ(GameManger�� �̿��Ͽ� �̵� ����, ī�޶� ����, ���콺 Ȱ��ȭ)
    private void OpenWindow()
    {
        isOpen = true;
        GameManager.isOpenShop = true;
        base_Ui.SetActive(true);
    }

    // ���� �ݱ�
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

    // ��ư�� Ŭ���� ���� ������ ȹ�� �Լ�
    // ����
    public void ClickButtonAxe()
    {
        theInventory.AcquireItem(items[0], 1);
    }
    // ���
    public void ClickButtonPickAxe()
    {
        theInventory.AcquireItem(items[1], 1);
    }
    // ��
    public void ClickButtonGun()
    {
        theInventory.AcquireItem(items[2], 1);
    }
    // ź��
    public void ClickButtonBullet()
    {
        theGun.carryBulletCount += 30;
    }
}
