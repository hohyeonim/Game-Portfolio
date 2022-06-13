using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twig : MonoBehaviour
{
    [SerializeField]
    private int hp; // 채력

    [SerializeField]
    private float destroyTime; // 이펙트 삭제 시간

    // 나뭇가지 아이템
    [SerializeField]
    private GameObject go_little_twig;

    // 타격 이펙트(나뭇잎 휘날림)
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    // 회전값 변수(원래 각도, 휘어짐 각도, 현재 각도)
    private Vector3 originRot;
    private Vector3 wantedRot;
    private Vector3 currentRot;

    // 필요한 사운드 이름(맞았을 때, 부서졌을 때)
    [SerializeField]
    private string hit_Sound;
    [SerializeField]
    private string broken_Sound;

    // 퀘스트 이벤트 시스템
    public UnityEngine.Events.UnityEvent onTwig;

    void Start()
    {
        originRot = transform.rotation.eulerAngles;
        currentRot = originRot;
    }

    // 나뭇가지 데미지 입었을 때(twig main 함수)
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

    // 나뭇가지가 공격 받았을 때
    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_Sound);

        GameObject clone = Instantiate(go_hit_effect_prefab, gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f), Quaternion.identity);

        Destroy(clone, destroyTime);
    }

    // 나뭇가지가 공격 받았을 때 휘어지는 코루틴(_target : player)
    IEnumerator HitSwayCoroutine(Transform _target)
    {
        // 서로 바라보는 방향 위치(player위치 - Twig 위치)
        Vector3 direction = (_target.position - transform.position).normalized;

        // 방향 각도
        Vector3 rotationDir = Quaternion.LookRotation(direction).eulerAngles;

        CheckDirection(rotationDir);

        // 나뭇가지 꺾임
        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.25f);
            transform.rotation = Quaternion.Euler(currentRot);

            yield return null;
        }

        wantedRot = originRot;

        // 나뭇가지 원래 위치로 되돌아옴
        while (!CheckThreshold())
        {
            currentRot = Vector3.Lerp(currentRot, wantedRot, 0.15f);
            transform.rotation = Quaternion.Euler(currentRot);

            yield return null;
        }
    }

    // currentRot이 wantedRot과 거의 가까워 졌는지 체크
    private bool CheckThreshold()
    {
        // 절대값으로 wantedRot - currentRot값이 0.5f에 가까워 졌을 때 true 반환
        if (Mathf.Abs(wantedRot.x - currentRot.x) <= 0.5f && Mathf.Abs(wantedRot.z - currentRot.z) <= 0.5f)
        {
            return true;
        }

        return false;
    }

    // HitSwayCoroutine에 있는 방향 각도를 체크하여 현재 방향 y값에 반대 방향을 구하기 위한 함수
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

    // 나뭇가지가 피0이 되면 삭제되고 그 위치에 나뭇가지 아이템 생성
    private void Destruction()
    {
        SoundManager.instance.PlaySE(broken_Sound);

        // 나뭇가지 아이템 2개 생성
        GameObject clone1 = Instantiate(go_little_twig, gameObject.GetComponent<BoxCollider>().bounds.center + (Vector3.up * 0.5f), Quaternion.identity);
        GameObject clone2 = Instantiate(go_little_twig, gameObject.GetComponent<BoxCollider>().bounds.center - (Vector3.up * 0.5f), Quaternion.identity);

        Destroy(clone1, destroyTime);
        Destroy(clone2, destroyTime);

        Destroy(gameObject);

        onTwig.Invoke();
    }
}
