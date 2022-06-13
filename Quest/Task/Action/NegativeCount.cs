using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/NegativeCount", fileName = "NegativeCount")]
public class NegativeCount : TaskAction
{
    // 들어온 성공 값이 음수일 때만 Count
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // 들어온 성공 값이 음수일 때만 더해주고, 아니면 현재 값 그대로 반환
        return successCount < 0 ? currentSuccess - successCount : currentSuccess;
    }
}
