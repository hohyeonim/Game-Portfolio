 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/SimpleSet", fileName = "simple set")]
public class SimpleSet : TaskAction
{
    // ������ ���� ���� ���� ���� ���� ���� ����
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // ���� �� ��ȯ
        return successCount;
    }
}
