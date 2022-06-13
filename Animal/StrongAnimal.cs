using System.Collections;
using UnityEngine;

public class StrongAnimal : Animal
{

    [SerializeField]
    protected int attackDamage; // 공격 데미지.
    [SerializeField]
    protected float attackDelay; // 공격 딜레이.
    [SerializeField]
    protected LayerMask targetMask; // 타겟 마스크.

    [SerializeField]
    protected float ChaseTime; // 총 추격 시간
    protected float currentChaseTime; // 계산.
    [SerializeField]
    protected float ChaseDelayTime; // 추격 딜레이.

    // 추적
    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        destination = _targetPos;
        nav.speed = runSpeed;
        isRunning = true;
        anim.SetBool("Running", isRunning);
        nav.SetDestination(destination);
    }

    // 공격 당하면 데미지를 입고 추적 시작
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
            Chase(_targetPos);
    }

    // 추적 코루틴(일정 시간 추적)
    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            // 충분히 가까이 있고,
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                if (theViewAngle.View()) // 눈 앞에 있으면
                {
                    StartCoroutine(AttackCoroutine()); // 공격
                }
            }
            yield return new WaitForSeconds(ChaseDelayTime); // 공격 코루틴 실행 후 바로 추적하지 않고 일정 딜레이 시간을 준다.
            currentChaseTime += ChaseDelayTime;
        }

        isChasing = false;
        isRunning = false;
        anim.SetBool("Running", isRunning);
        nav.ResetPath();
    }

    // 공격 코루틴(추적 딜레이 및 공격 딜레이를 주고 충돌 정보를 계속 받아와 충돌 범위 안에 들어오면 공격 적중하고 데미지를 입음 공격 중 충돌 범위 밖으로 벗어나면 공격 실패)
    protected IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        nav.ResetPath();
        currentChaseTime = ChaseTime;
        yield return new WaitForSeconds(0.5f); 
        transform.LookAt(new Vector3(theViewAngle.GetTargetPos().x, 0f, theViewAngle.GetTargetPos().z)); // 타겟(Player)을 바라본다.
        anim.SetTrigger("Attack"); // 공격 애니메이션 실행
        yield return new WaitForSeconds(0.5f);
        RaycastHit _hit;
        // 충돌 정보에 설정한 targetMask가 있으면 데미지를 준다.
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out _hit, 3, targetMask))
        {
            Debug.Log("플레이어 적중!!");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("플레이어 빗나감!!");
        }

        yield return new WaitForSeconds(attackDelay); // 공격 후 딜레이 시간을 준다.
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine()); // 추적 코루틴 실행
    }
}
