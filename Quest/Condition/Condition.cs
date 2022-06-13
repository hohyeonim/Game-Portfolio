using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����Ʈ�� ���� �� �ִ� ���� ����
public abstract class Condition : ScriptableObject
{
    [SerializeField]
    private string description;

    public abstract bool IsPass(Quest quest);
}
