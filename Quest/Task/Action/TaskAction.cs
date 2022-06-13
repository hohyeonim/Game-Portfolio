using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추상 클래스
public abstract class TaskAction : ScriptableObject
{
    // 추상 함수
    public abstract int Run(Task task, int currentSuccess, int successCount);
}
