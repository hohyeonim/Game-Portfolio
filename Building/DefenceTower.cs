using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceTower : MonoBehaviour
{
    [SerializeField]
    private string towerName; // 방어 타워의 이름
    [SerializeField]
    private float range; // 방어타워의 사정 거리
    [SerializeField]
    private int damage; // 방어타워 공격력
    [SerializeField]
    private float rateOfAccurasy; // 정확도
    [SerializeField]
    private float rateOfFire; // 연사속도
    private float currentRateOfFire; // 연사속도 계산
    [SerializeField]
    private float viewAngle; // 시야각
    [SerializeField]
    private float spinSpeed; // 포신 회전 속도
    [SerializeField]
    private LayerMask layerMask; // 움직이는 대상만 타겟으로 지정(플레이어, 동물, NPC)
    [SerializeField]
    private Transform tf_TopGun; // 방어 타워의 포탑
    [SerializeField]
    private ParticleSystem particle_MuzzleFlash; // 총구 섬광
    [SerializeField]
    private GameObject go_HitEffect_Prefab; // 적중 효과 이펙트

    private RaycastHit hitInfo; // 광선 충돌 객체의 정보 저장
    private Animator anim;
    private AudioSource theAudio;

    private bool isFindTarget = false; // 적 타겟 발견 시 true;
    private bool isAttack = false; // 총구 방향과 적 방향이 일치할 시 true

    private Transform tf_Target; // 현재 설정된 타겟

    [SerializeField]
    private AudioClip sound_Fire;

    void Start()
    {
        theAudio = GetComponent<AudioSource>();
        theAudio.clip = sound_Fire;
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Spin();
        SearchEnemy();
        LookTarget();
        Attack();
    }

    // 총 회전 함수
    private void Spin()
    {
        if (!isFindTarget && !isAttack)
        {
            Quaternion _spin = Quaternion.Euler(0f, tf_TopGun.eulerAngles.y + (1f * spinSpeed * Time.deltaTime), 0f);
            tf_TopGun.rotation = _spin;
        }
    }

    // 시야 안에 적을 찾는 함수
    private void SearchEnemy()
    {
        Collider[] _targets = Physics.OverlapSphere(tf_TopGun.position, range, layerMask);

        for (int i = 0; i < _targets.Length; i++)
        {
            Transform _targetTf = _targets[i].transform;

            if (_targetTf.name == "Player")
            {
                // 거리 및 각도 임시 변수
                Vector3 _direction = (_targetTf.position - tf_TopGun.position).normalized;
                float _angle = Vector3.Angle(_direction, tf_TopGun.forward);

                // 해당 각도안에 Player가 있으면 공격 실행
                if (_angle < viewAngle * 0.5f)
                {
                    tf_Target = _targetTf;
                    isFindTarget = true;

                    if (_angle < 5f)
                        isAttack = true;
                    else
                        isAttack = false;
                    return;
                }
            }
        }
        tf_Target = null;
        isAttack = false;
        isFindTarget = false;
    }

    // 타겟에 시야 고정
    private void LookTarget()
    {
        if (isFindTarget)
        {
            Vector3 _direction = (tf_Target.position - tf_TopGun.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            Quaternion _rotation = Quaternion.Lerp(tf_TopGun.rotation, _lookRotation, 0.2f);
            tf_TopGun.rotation = _rotation;
        }
    }

    // 공격
    private void Attack()
    {
        if (isAttack)
        {
            // 연사 속도 계산
            currentRateOfFire += Time.deltaTime;
            if (currentRateOfFire >= rateOfFire)
            {
                currentRateOfFire = 0;
                anim.SetTrigger("Fire");
                theAudio.Play();
                particle_MuzzleFlash.Play();

                // 충돌 정보(거리가 멀어 질 수록 총 정확도가 낮아짐)
                if (Physics.Raycast(tf_TopGun.position,
                                   tf_TopGun.forward + new Vector3(Random.Range(-1, 1f) * rateOfAccurasy, Random.Range(-1, 1f) * rateOfAccurasy, 0f),
                                   out hitInfo, range, layerMask))
                {
                    GameObject _temp = Instantiate(go_HitEffect_Prefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                    Destroy(_temp, 1f);

                    // Player 데미지
                    if (hitInfo.transform.name == "Player")
                    {
                        hitInfo.transform.GetComponent<StatusController>().DecreaseHP(damage);
                    }
                }
            }
        }
    }
}
