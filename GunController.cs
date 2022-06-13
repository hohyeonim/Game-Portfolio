using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    // Ȱ��ȭ ����
    public static bool isActivate = false;

    // ���� ������ ��
    [SerializeField]
    private Gun currentGun;

    // ���� �ӵ�
    private float currentFireRate;

    // ���� ����
    private bool isReload = false;
    [HideInInspector]
    private bool isFineSightMode = false;
     
    // ���� ������ ��
    private Vector3 originPos;

    // ȿ���� ���
    private AudioSource audioSource;

    // ������ �浹 ���� �޾ƿ�
    private RaycastHit hitInfo;
    [SerializeField]
    private LayerMask layerMask;

    // �ʿ��� ������Ʈ
    [SerializeField]
    private Camera theCam;
    private Crosshair theCrosshair;

    // �ǰ� ����Ʈ
    [SerializeField]
    private GameObject hit_effect_prefab;

    private void Start()
    {
        originPos = Vector3.zero;
        audioSource = GetComponent<AudioSource>();
        theCrosshair = FindObjectOfType<Crosshair>();

        
    }

    void Update()
    {
        if (isActivate && GameManager.canPlayerAttack)
        {
            GunFireRateCalc();
            TryFire();
            TryReload();
            TryFineSight();
        }
    }

    // ���� �ӵ� ����
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
        {
            currentFireRate -= Time.deltaTime;
        }
    }

    // �߻� �õ�
    private void TryFire()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    // �߻� �� ���
    private void Fire()
    {
        if (!isReload)
        {
            if (currentGun.currentBulletCount > 0)
            {
                Shoot();
            }
            else
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    // �߻� �� ���
    private void Shoot()
    {
        theCrosshair.FireAnimation();
        currentGun.currentBulletCount--; // ź�� �Ҹ�
        currentFireRate = currentGun.fireRate; // ���� �ӵ� ����
        PlaySE(currentGun.fire_Sound);
        currentGun.muzzleFlash.Play();
        Hit();
        StopAllCoroutines();
        // �ѱ� �ݵ� �ڷ�ƾ ����
        StartCoroutine(RetroActionCoroutine());
    }

    private void Hit()
    {
        // Raycast�� �̿��Ͽ� ���� ���� ī�޶� ����� ��Ȯ��, �����Ÿ��� �̿��Ͽ� �浹 ������ �޾ƿ�
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward + 
            new Vector3(Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy)
            , Random.Range(-theCrosshair.GetAccuracy() - currentGun.accuracy, theCrosshair.GetAccuracy() + currentGun.accuracy)
            ,0)
            , out hitInfo, currentGun.range, layerMask))
        {
            // ���� �����̰ų� ���� ������ �浹 ������ ������ �ش� ������ ������� ������.
            if (hitInfo.transform.tag == "WeakAnimal")
            {
                SoundManager.instance.PlaySE("Animal_Hit");
                hitInfo.transform.GetComponent<WeakAnimal>().Damage(currentGun.damage, transform.position);
            }
            else if (hitInfo.transform.tag == "StrongAnimal")
            {
                SoundManager.instance.PlaySE("Animal_Hit");
                hitInfo.transform.GetComponent<StrongAnimal>().Damage(currentGun.damage, transform.position);
            }

            // �浹 ������ �Ѿ� ����Ʈ ����
            GameObject clone = Instantiate(hit_effect_prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            Destroy(clone, 2f);
        }
    }

    // ������ �õ�
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentGun.currentBulletCount < currentGun.reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // ������ ���
    public void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }
     
    // ������
    IEnumerator ReloadCoroutine()
    {
        // ź���� 0���� Ŭ ��
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;

            currentGun.anim.SetTrigger("Reload");

            // ���� ������ �ִ� ź���� �� ź�� ������ �������� ���� ������ �ִ� ź���� 0�� �ȴ�.(������ �ϱ� �� ������ �ִ� ź���� �� ź�� ������ ���� ���� �ٽ� ź���� ������)
            currentGun.carryBulletCount += currentGun.currentBulletCount;
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);

            // ������ �ִ� ź���� �������� �ʿ��� ź�� ������ ũ�� ���� ź�࿡ ������ �ʿ� ź���� �ְ� �� ������ �ִ� ź�࿡ ������ ź�� ������ ���ش�
            if (currentGun.carryBulletCount >= currentGun.reloadBulletCount)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletCount;
                currentGun.carryBulletCount -= currentGun.reloadBulletCount;
            }
            else // ������ �ִ� ź���� �������� �ʿ��� ź�� ������ ������ ������ �ִ� ź�� ���� ��� ������ �ϰ� �� ������ �ִ� ź�� ���� 0�� �ȴ�.
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }

            isReload = false;
        }
        else // ź���� ���� ��
        {
            Debug.Log("������ �Ѿ��� �����ϴ�.");
        }

    }

    // ������ �õ�
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }

    // ������ ���
    public void CancelFineSight()
    {
        if (isFineSightMode)
        {
            FineSight();
        }
    }

    // ������ ���� ����
    private void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        currentGun.anim.SetBool("FineSightMode", isFineSightMode);
        theCrosshair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeActivateCoroutine());
        }
    }

    // ������ Ȱ��ȭ
    IEnumerator FineSightActivateCoroutine()
    {
        while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    // ������ ��Ȱ��ȭ
    IEnumerator FineSightDeActivateCoroutine()
    {
        while (currentGun.transform.localPosition != originPos)
        {
            currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    // �ݵ� �ڷ�ƾ
    IEnumerator RetroActionCoroutine()
    {
        // �ӽ� ����(�Ϲ� ��� �ִ� �ݵ�, ������ ��� �ִ� �ݵ�)
        Vector3 recoilBack = new Vector3(currentGun.retroActionFineSightForce, originPos.y, originPos.z);
        Vector3 retroActionRecoilBack = new Vector3(currentGun.retroActionFineSightForce, currentGun.fineSightOriginPos.y, currentGun.fineSightOriginPos.z);

        if (!isFineSightMode) // �Ϲ� ��� ���
        {
            // ���� �� ��ġ�� ���� ��ġ�� �ǵ���(��� �ݵ��� �̷�������)
            currentGun.transform.localPosition = originPos;

            // �Ϲ� ��� ��� �ݵ� ����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, recoilBack, 0.4f);
                yield return null;
            }

            //����ġ
            while (currentGun.transform.localPosition != originPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, originPos, 0.1f);
                yield return null;
            }
        }
        else // ������ ��� ���
        {
            currentGun.transform.localPosition = currentGun.fineSightOriginPos;

            // ������ ��� �ݵ� ����
            while (currentGun.transform.localPosition.x <= currentGun.retroActionFineSightForce - 0.02f)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, retroActionRecoilBack, 0.4f);
                yield return null;
            }

            //����ġ
            while (currentGun.transform.localPosition != currentGun.fineSightOriginPos)
            {
                currentGun.transform.localPosition = Vector3.Lerp(currentGun.transform.localPosition, currentGun.fineSightOriginPos, 0.1f);
                yield return null;
            }
        }
    }

    // ���� ���
    private void PlaySE(AudioClip _clip)
    {
        audioSource.clip = _clip;
        audioSource.PlayOneShot(audioSource.clip);
    }

    public Gun GetGun()
    {
        return currentGun;
    }

    public bool GetFineSightMode()
    {
        return isFineSightMode;
    }

    public void GunChange(Gun _gun)
    {
        if (WeaponManager.currentWeapon != null)
        {
            WeaponManager.currentWeapon.gameObject.SetActive(false);
        }

        currentGun = _gun;
        WeaponManager.currentWeapon = currentGun.GetComponent<Transform>();
        WeaponManager.currentWeaponAnim = currentGun.anim;

        currentGun.transform.localPosition = Vector3.zero;
        currentGun.gameObject.SetActive(true);
        isActivate = true;
    }
}
