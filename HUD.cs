using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    // �ʿ��� ������Ʈ
    [SerializeField]
    private GunController theGunController;
    private Gun currentGun;

    // �ʿ��ϸ� HUD ȣ��, �ʿ������ HUD ��Ȱ��ȭ
    [SerializeField]
    private GameObject go_BulletHUD;

    // �Ѿ� ���� �ؽ�Ʈ UI �ݿ�
    [SerializeField]
    private Text[] text_Bullet;

    [SerializeField]
    private GameObject go_BaseUi;

    void Update()
    {
        CheckBullet();
        GunUI();
    }

    // �Ѿ� UI Ȱ��ȭ/��Ȱ��ȭ
    private void GunUI()
    {
        if (GunController.isActivate)
        {
            go_BaseUi.SetActive(true);
        }
        else
        {
            go_BaseUi.SetActive(false);
        }
    }

    // ź�� Ȯ��(�� �Ѿ� ����, źâ�� �ִ�� ���� �Ѿ� ����, ���� źâ�� �ִ� �Ѿ� ����)
    private void CheckBullet()
    {
        currentGun = theGunController.GetGun();
        text_Bullet[0].text = currentGun.carryBulletCount.ToString();
        text_Bullet[1].text = currentGun.reloadBulletCount.ToString();
        text_Bullet[2].text = currentGun.currentBulletCount.ToString();
    }
}
