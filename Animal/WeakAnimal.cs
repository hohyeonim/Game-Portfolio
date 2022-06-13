using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    // �÷��̾� �ݴ� �������� ����
    public void Run(Vector3 _targetPos)
    {
        // �ݴ� ��ġ ��� - x�� : ���� x�� ��ġ - �÷��̾� x�� ��ġ, z�� : ���� z�� ��ġ - �÷��̾� z�� ��ġ
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;

        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        nav.speed = runSpeed;
        anim.SetBool("Running", isRunning);
    }

    // ������ ������ �� �������� �԰� ���� ������ ����
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
        {
            Run(_targetPos);
        }
    }
}