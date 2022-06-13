using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskTarget : ScriptableObject
{
    // Target�� �ܺη� ������ �� �ִ� ValueProperty�� �߻� ���·� ����
    public abstract object Value { get; }

    // QuestSystem�� ����� Target�� Task�� ������ Target�� ������ Ȯ��
    public abstract bool IsEqual(object target);
}
