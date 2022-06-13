using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitialSuccessValue : ScriptableObject
{
    // Task의 초기 성공 값을 정함
    // 예) 내 현재 렙, 현재 필드에 몬스터 수
    public abstract int GetValue(Task task);
}
