using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    [SerializeField]
    private int hp; // 바위의 체력

    [SerializeField]
    private float destroyTime; // 파편 제거 시간

    [SerializeField]
    private SphereCollider col; // 구체 컬라이더

    // 필요한 게임 오브젝트
    [SerializeField]
    private GameObject go_rock; //일반 바위
    [SerializeField]
    private GameObject go_debris; // 깨진 바위
    [SerializeField]
    private GameObject go_effect_prefabs; // 채굴 이펙트(돌 파편)
    [SerializeField]
    private GameObject go_rock_item_prefabs; // 돌맹이 아이템

    // 돌맹이 아이템 등장 개수
    [SerializeField]
    private int count;


    // 필요한 사운드 이름
    [SerializeField]
    private string strike_Sound;
    [SerializeField]
    private string destroy_Sound;

    // 퀘스트 이벤트 시스템
    public UnityEngine.Events.UnityEvent onRock;

    // 채광 main 함수
    public void Mining()
    {
        SoundManager.instance.PlaySE(strike_Sound);

        // 채광 시 바위 파편 생성
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
        {
            Destruction();
        }
    }

    // 바위 깨짐(사운드 및 돌맹이 아이템 생성)
    private void Destruction()
    {
        SoundManager.instance.PlaySE(destroy_Sound);

        col.enabled = false;

        // 돌맹이 아이템 생성
        for (int i = 0; i < count; i++)
        {
            Instantiate(go_rock_item_prefabs, go_rock.transform.position, Quaternion.identity);
        }

        // 바위 삭제
        Destroy(go_rock);

        // 깨진 바위 생성 및 삭제
        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);

        onRock.Invoke();
    }
}
