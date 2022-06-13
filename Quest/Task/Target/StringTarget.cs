using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/String", fileName = "Target_")]
public class StringTarget : TaskTarget
{
    [SerializeField]
    private string value; // ���� ���� value ����

    // �߻� Property�� value�� return �ϵ��� override
    public override object Value => value;

    // ���� ������ Value�� Pig��� ���ڿ��̰�
    // ���� target�� Pig��� ���ڿ��̶�� true�� return�Ǿ�
    // �� Task�� ��ǥ�� �ϴ� Target�� �´ٴ� ���� �Ʒ���
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
