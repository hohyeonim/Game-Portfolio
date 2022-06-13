using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/NegativeCount", fileName = "NegativeCount")]
public class NegativeCount : TaskAction
{
    // ���� ���� ���� ������ ���� Count
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // ���� ���� ���� ������ ���� �����ְ�, �ƴϸ� ���� �� �״�� ��ȯ
        return successCount < 0 ? currentSuccess - successCount : currentSuccess;
    }
}
