using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : WeakAnimal
{

    protected override void Update()
    {
        base.Update();
        if (theViewAngle.View() && !isDead)
        {
            Run(theViewAngle.GetTargetPos());
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
        int _random = Random.Range(0, 2); // 대기, 풀뜯기, 두리번, 걷기

        if (_random == 0)
        {
            Wait();
        }
        else if (_random == 1)
        {
            TryWalk();
        }

    }

    // 대기
    private void Wait()
    {
        currentTime = waitTime;
    }
}
