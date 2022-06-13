using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/GameObject", fileName = "Target_")]
public class GameObjectTarget : TaskTarget
{
    [SerializeField]
    private GameObject value;

    public override object Value => value;

    // ���� ������ Valuer���� Pig��� GameObject�� �̸��� ���ԵǾ� �ִ���
    // ���� target�� (Clone)Pig(1) ��� GameObject����� true�� return�Ǿ�
    // �� Task�� ��ǥ�� �ϴ� Target�� �´ٴ� ���� �Ʒ���
    public override bool IsEqual(object target)
    {
        var targetAsGameObject = target as GameObject;

        if (targetAsGameObject == null)
        {
            return false;
        }

        // �Ȱ��� GameObject Prefab�� ����Ƽ�� ���� ������ �ڿ� (1) ~ �̷��� �ٰ� �ǰų� Instantiate �Ҷ��� �ڿ� (clone)�� �ٰ� �ǹǷ�
        // ���ԵǴ� �̸��� ã�� �� �ִ� Contains�� ��� (��.. ã�� string�� Pig �̸� - Pig(1) or pig(clone)�� ã�� �ش�.
        return targetAsGameObject.name.Contains(value.name);
    }
}
