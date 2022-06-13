using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/ContinuousCount", fileName = "ContinuousCount")]
public class ContinuousCount : TaskAction
{
    // 성공 값을 양수가 들어오면 Count, 음수가 들어오면 현재 성공 횟수를 0으로 초기화
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return successCount > 0 ? currentSuccess + successCount : 0;
    }
}
