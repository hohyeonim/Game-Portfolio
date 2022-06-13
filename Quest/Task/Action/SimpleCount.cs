using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 성공 Count에 받은 성공 Count를 더해서 전달
[CreateAssetMenu(menuName = "Quest/Task/Action/SimpleCount", fileName = "simple Count")]
public class SimpleCount : TaskAction
{
    // 추상 함수 구현
    public override int Run(Task task, int currentSuccess, int successCount)
    {
        return currentSuccess + successCount;
    }
}
