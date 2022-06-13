using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField]
    private int hp; // ä��

    [SerializeField]
    private float destroyTime; // ����Ʈ ���� �ð�

    // �������� ������
    [SerializeField]
    private GameObject go_little_twig;

    // Ÿ�� ����Ʈ(������ �ֳ���)
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    // ȸ���� ����(���� ����, �־��� ����, ���� ����)
    private Vector3 originRot;
    private Vector3 wantedRot;
    private Vector3 currentRot;

    // �ʿ��� ���� �̸�(�¾��� ��, �μ����� ��)
    [SerializeField]
    private string hit_Sound;
    [SerializeField]
    private string broken_Sound;

    // ����Ʈ �̺�Ʈ �ý���
    public UnityEngine.Events.UnityEvent onTwig;

    void Start()
    {
        originRot = transform.rotation.eulerAngles;
        currentRot = originRot;
    }

    // �������� ������ �Ծ��� ��(twig main �Լ�)
    public void Damage(Transform _playerTf)
    {
        hp--;

        Hit();

        StartCoroutine(HitSwayCoroutine(_playerTf));

        if (hp <= 0)
        {
            Destruction();
        }
    }

    // ���������� ���� �޾��� ��
    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_Sound);

        GameObject clone = Instantiate(go_hit_effect_prefab, gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f), Quaternion.identity);

        Destroy(clone, destroyTime);
    }

    // ���������� ���� �޾��� �� �־����� �ڷ�ƾ(_target : player)
    IEnumerator HitSwayCoroutine(Transform _target)
    {
        // ���� �ٶ󺸴� ���� ��ġ(player��ġ - Twig ��ġ)
        Vector3 direction = (_target.position - transform.position).normalized;

        // ���� ����
        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;

        CheckDirection(rotationDir);

        // �������� ����
        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);

            yield return null;
        }

        wantedRot = originRot;

        // �������� ���� ��ġ�� �ǵ��ƿ�
        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.15f);
            transform.rotation = Quaternion.Euler(currentRot);

            yield return null;
        }
    }

    // currentRot�� wantedRot�� ���� ����� ������ üũ
    private bool CheckThreshold()
    {
        // ���밪���� wantedRot - currentRot���� 0.5f�� ����� ���� �� true ��ȯ
        if (Mathf.Abs(wantedRot.x - currentRot.x) <= 0.5f && Mathf.Abs(wantedRot.z - currentRot.z) <= 0.5f)
        {
            return true;
        }

        return false;
    }

    // HitSwayCoroutine�� �ִ� ���� ������ üũ�Ͽ� ���� ���� y���� �ݴ� ������ ���ϱ� ���� �Լ�
    private void CheckDirection(Vector3 _rotationDir)
    {
        Debug.Log(_rotationDir);

        if (_rotationDir.y > 180)
        {
            if (_rotationDir.y > 300)
            {
                wantedRot = new Vector3(-50f, 0f, -50f);
            }
            else if (_rotationDir.y > 240)
            {
                wantedRot = new Vector3(0f, 0f, -50f);
            }
            else
            {
                wantedRot = new Vector3(50f, 0f, -50f);
            }
        }
        else if (_rotationDir.y <= 180)
        {
            if (_rotationDir.y < 60)
            {
                wantedRot = new Vector3(-50f, 0f, -50f);
            }
            else if (_rotationDir.y > 120)
            {
                wantedRot = new Vector3(0f, 0f, 50f);
            }
            else
            {
                wantedRot = new Vector3(50f, 0f, 50f);
            }
        }
    }

    // ���������� ��0�� �Ǹ� �����ǰ� �� ��ġ�� �������� ������ ����
    private void Destruction()
    {
        SoundManager.instance.PlaySE(broken_Sound);

        // �������� ������ 2�� ����
        GameObject clone1 = Instantiate(go_little_twig, gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f), Quaternion.identity);
        GameObject clone2 = Instantiate(go_little_twig, gameObject.GetComponent<BoxCollider>().bounds.center - (Vector3.up * 0.5f), Quaternion.identity);

        Destroy(clone1, destroyTime);
        Destroy(clone2, destroyTime);

        Destroy(gameObject);

        onTwig.Invoke();
    }
}
