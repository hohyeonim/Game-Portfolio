using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InitialSuccessValue : ScriptableObject
{
    // Task�� �ʱ� ���� ���� ����
    // ��) �� ���� ��, ���� �ʵ忡 ���� ��
    public abstract int GetValue(Task task);
}
