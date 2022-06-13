using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트를 받을 수 있는 선행 조건
public abstract class Condition : ScriptableObject
{
    [SerializeField]
    private string description;

    public abstract bool IsPass(Quest quest);
}
