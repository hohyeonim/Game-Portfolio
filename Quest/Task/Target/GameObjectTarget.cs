using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Task/Target/GameObject", fileName = "Target_")]
public class GameObjectTarget : TaskTarget
{
    [SerializeField]
    private GameObject value;

    public override object Value => value;

    // 내가 설정한 Valuer값이 Pig라는 GameObject에 이름이 포함되어 있는지
    // 들어온 target이 (Clone)Pig(1) 라는 GameObject에라면 true가 return되어
    // 이 Task가 목표로 하는 Target이 맞다는 것을 아려줌
    public override bool IsEqual(object target)
    {
        var targetAsGameObject = target as GameObject;

        if (targetAsGameObject == null)
        {
            return false;
        }

        // 똑같은 GameObject Prefab을 유니티에 직접 넣으면 뒤에 (1) ~ 이렇게 붙게 되거나 Instantiate 할때도 뒤에 (clone)이 붙게 되므로
        // 포함되는 이름을 찾을 수 있는 Contains를 사용 (예.. 찾는 string이 Pig 이면 - Pig(1) or pig(clone)도 찾아 준다.
        return targetAsGameObject.name.Contains(value.name);
    }
}
