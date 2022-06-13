 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Action/SimpleSet", fileName = "simple set")]
public class SimpleSet : TaskAction
{
    // 스스로 들어온 성공 값을 현재 성공 값에 대입
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        // 들어온 값 반환
        return successCount;
    }
}
