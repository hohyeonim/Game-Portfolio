using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : WeakAnimal
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
        int _random = Random.Range(0, 3); // ���, Ǯ���, �ȱ�

        if (_random == 0)
        {
            Wait();
        }
        else if (_random == 1)
        {
            Eat();
        }
        else if (_random == 2)
        {
            TryWalk();
        }

    }

    // ���
    private void Wait()
    {
        currentTime = waitTime;
    }

    // Ǯ���
    private void Eat()
    {
        currentTime = waitTime;
        anim.SetTrigger("Eat");
    }
}
