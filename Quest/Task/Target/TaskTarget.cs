using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskTarget : ScriptableObject
{
    // Target을 외부로 가져올 수 있는 ValueProperty를 추상 형태로 만듬
    public abstract object Value { get; }

    // QuestSystem에 보고된 Target이 Task에 설정한 Target과 같은지 확인
    public abstract bool IsEqual(object target);
}
