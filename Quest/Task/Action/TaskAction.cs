using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �߻� Ŭ����
public abstract class TaskAction : ScriptableObject
{
    // �߻� �Լ�
    public abstract int Run(Task task, int currentSuccess, int successCount);
}
