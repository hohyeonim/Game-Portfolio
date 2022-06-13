using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string value; // 값을 받을 value 선언

    // 추상 Property도 value를 return 하도록 override
    public override object Value => value;

    // 내가 설정한 Value가 Pig라는 문자열이고
    // 들어온 target이 Pig라는 문자열이라면 true가 return되어
    // 이 Task가 목표로 하는 Target이 맞다는 것을 아려줌
    public override bool IsEqual(object target)
    {
        string targetAsString = target as string;

        if (targetAsString == null)
        {
            return false;
        }

        return value == targetAsString;
    }
}
