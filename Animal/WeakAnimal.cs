using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakAnimal : Animal
{
    // 플레이어 반대 방향으로 도망
    public void Run(Vector3 _targetPos)
    {
        // 반대 위치 계산 - x값 : 동물 x값 위치 - 플레이어 x값 위치, z값 : 동물 z값 위치 - 플레이어 z값 위치
        destination = new Vector3(transform.position.x - _targetPos.x, 0f, transform.position.z - _targetPos.z).normalized;

        currentTime = runTime;
        isWalking = false;
        isRunning = true;
        nav.speed = runSpeed;
        anim.SetBool("Running", isRunning);
    }

    // 공격을 당했을 때 데미지를 입고 죽지 않으면 도망
    public override void Damage(int _dmg, Vector3 _targetPos)
    {
        base.Damage(_dmg, _targetPos);
        if (!isDead)
        {
            Run(_targetPos);
        }
    }
}
