using System.Collections;
using UnityEngine;

public class StrongAnimal : Animal
{

    [SerializeField]
    protected int attackDamage; // ���� ������.
    [SerializeField]
    protected float attackDelay; // ���� ������.
    [SerializeField]
    protected LayerMask targetMask; // Ÿ�� ����ũ.

    [SerializeField]
    protected float ChaseTime; // �� �߰� �ð�
    protected float currentChaseTime; // ���.
    [SerializeField]
    protected float ChaseDelayTime; // �߰� ������.

    // ����
    public void Chase(Vector3 _targetPos)
    {
        isChasing = true;
        destination = _targetPos;
        nav.speed = runSpeed;
        isRunning = true;
        anim.SetBool("Running", isRunning);
        nav.SetDestination(destination);
    }

    // ���� ���ϸ� �������� �԰� ���� ����
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
            Chase(_targetPos);
    }

    // ���� �ڷ�ƾ(���� �ð� ����)
    protected IEnumerator ChaseTargetCoroutine()
    {
        currentChaseTime = 0;

        while (currentChaseTime < ChaseTime)
        {
            Chase(theViewAngle.GetTargetPos());
            // ����� ������ �ְ�,
            if (Vector3.Distance(transform.position, theViewAngle.GetTargetPos()) <= 3f)
            {
                if (theViewAngle.View()) // �� �տ� ������
                {
                    StartCoroutine(AttackCoroutine()); // ����
                }
            }
            yield return new WaitForSeconds(ChaseDelayTime); // ���� �ڷ�ƾ ���� �� �ٷ� �������� �ʰ� ���� ������ �ð��� �ش�.
            currentChaseTime += ChaseDelayTime;
        }

        isChasing = false;
        isRunning = false;
        anim.SetBool("Running", isRunning);
        nav.ResetPath();
    }

    // ���� �ڷ�ƾ(���� ������ �� ���� �����̸� �ְ� �浹 ������ ��� �޾ƿ� �浹 ���� �ȿ� ������ ���� �����ϰ� �������� ���� ���� �� �浹 ���� ������ ����� ���� ����)
    protected IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        nav.ResetPath();
        currentChaseTime = ChaseTime;
        yield return new WaitForSeconds(0.5f); 
        transform.LookAt(new Vector3(theViewAngle.GetTargetPos().x, 0f, theViewAngle.GetTargetPos().z)); // Ÿ��(Player)�� �ٶ󺻴�.
        anim.SetTrigger("Attack"); // ���� �ִϸ��̼� ����
        yield return new WaitForSeconds(0.5f);
        RaycastHit _hit;
        // �浹 ������ ������ targetMask�� ������ �������� �ش�.
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out _hit, 3, targetMask))
        {
            Debug.Log("�÷��̾� ����!!");
            thePlayerStatus.DecreaseHP(attackDamage);
        }
        else
        {
            Debug.Log("�÷��̾� ������!!");
        }

        yield return new WaitForSeconds(attackDelay); // ���� �� ������ �ð��� �ش�.
        isAttacking = false;
        StartCoroutine(ChaseTargetCoroutine()); // ���� �ڷ�ƾ ����
    }
}
