using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : StrongAnimal
{
    protected override void Update()
    {
        base.Update();
        if (theViewAngle.View() && !isDead && !isAttacking)
        {
            StopAllCoroutines();
            StartCoroutine(ChaseTargetCoroutine());
        }
    }

    protected override void ReSet()
    {
        base.ReSet();
        RandomAction();
    }

    private void RandomAction()
    {
        RandomSound();
        int _random = Random.Range(0, 3); // 대기, 일어서기, 걷기

        if (_random == 0)
        {
            Wait();
        }
        else if (_random == 1)
        {
            TryWalk();
        }
        else if (_random == 2)
        {
            StandUp();
        }

    }

    // 대기
    private void Wait()
    {
        currentTime = waitTime;
    }

    private void StandUp()
    {
        currentTime = 6f;
        anim.SetTrigger("StandUp");
    }
}
