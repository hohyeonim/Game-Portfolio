using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : StrongAnimal
{
    protected override void Update()
    {
        base.Update();
        if (theViewAngle.View() && !isDead && !isAttacking)
        {
            StopAllCoroutines();
            StartCoroutine(ChaseTargetCoroutine());
        }

        if(isDead)
        {
            StopAllCoroutines();
            isRunning = false;
            isChasing = false;
        }
    }
}
