using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/PositiveCount", fileName = "positive count")]
public class PositiveCount : TaskAction
{
    // ���� ���� ���� ��� �϶� �� Count
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // ���� ���� 0���� ũ�� current ���� ���ؼ� ��ȯ, �ƴϸ� ���� �� �״�� ��ȯ
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;
    }
}
