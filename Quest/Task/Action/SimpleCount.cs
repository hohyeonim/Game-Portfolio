using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� ���� Count�� ���� ���� Count�� ���ؼ� ����
[CreateAssetMenu(menuName = "Quest/Task/Action/SimpleCount", fileName = "simple Count")]
public class SimpleCount : TaskAction
{
    // �߻� �Լ� ����
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }
}
