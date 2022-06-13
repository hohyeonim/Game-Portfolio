using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/PositiveCount", fileName = "positive count")]
public class PositiveCount : TaskAction
{
    // 들어온 성공 값이 양수 일때 만 Count
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // 성공 값이 0보다 크면 current 값에 더해서 반환, 아니면 현재 값 그대로 반환
        return successCount > 0 ? currentSuccess + successCount : currentSuccess;
    }
}
