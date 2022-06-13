using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    // 깎일 나무 조각들
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;

    // 통나무
    [SerializeField]
    private GameObject go_Log_prefabs;

    // 쓰러질 때의 랜덤으로 가해질 힘의 세기
    [SerializeField]
    private float force;

    // 자식 트리
    [SerializeField]
    private GameObject go_ChildTree;

    // 파편
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    // 파편 제거 시간
    [SerializeField]
    private float debrisDestryTime;

    // 나무 제거 시간
    [SerializeField]
    private float destroyTime;

    // 부모 트리가 파괴되면 캡슐 콜라이더 제거
    [SerializeField]
    private CapsuleCollider parentCol;

    // 자식 트리가 쓰러질 때 필요한 컴포넌트 활성화 및 중력 활성화
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    // 필요한 사운드
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;

    // 퀘스트 이벤트 시스템
    public UnityEngine.Events.UnityEvent onLog;

    // 벌목 main 함수
    public void Chop(Vector3 _pos, float angleY)
    {
        Hit(_pos);

        AngleCalc(angleY);

        if (CheckTreePieces())
        {
            return;
        }

        FallDownTree();
    }

    // 적중 이펙트
    private void Hit(Vector3 _pos)
    {
        SoundManager.instance.PlaySE(chop_sound);

        // 나무 파편 prefab 생성 및 삭제
        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestryTime);
    }

    // 각도에 따른 나무 조각 삭제
    private void AngleCalc(float _angleY)
    {
        if (0 <= _angleY && _angleY <= 70)
        {
            DestroyPiece(2);
        }
        else if (70 <= _angleY && _angleY <= 140)
        {
            DestroyPiece(3);
        }
        else if (140 <= _angleY && _angleY <= 210)
        {
            DestroyPiece(4);
        }
        else if (210 <= _angleY && _angleY <= 280)
        {
            DestroyPiece(0);
        }
        else if (280 <= _angleY && _angleY <= 360)
        {
            DestroyPiece(1);
        }
    }

    // 나무 조각 삭제
    private void DestroyPiece(int _num)
    {
        if (go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[_num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestryTime);
            Destroy(go_treePieces[_num].gameObject);
        }

    }

    // 벌목에 필요한 나무 조각들 체크
    private bool CheckTreePieces()
    {
        for (int i = 0; i < go_treePieces.Length; i++)
        {
            if (go_treePieces[i].gameObject != null)
            {
                return true;
            }
        }

        return false;
    }

    // 나무 쓰러짐 구현
    private void FallDownTree()
    {
        SoundManager.instance.PlaySE(falldown_sound);

        Destroy(go_treeCenter);

        parentCol.enabled = false;
        childCol.enabled = true;
        childRigid.useGravity = true;

        childRigid.AddForce(Random.Range(-force, force), 0f, Random.Range(-force, force));

        StartCoroutine(LogCoroutine());
    }

    // 코루틴 : 나무가 쓰러지면서 나무 윗부분은 사라지고 통나무 prefabs 3개 생성
    IEnumerator LogCoroutine()
    {
        yield return new WaitForSeconds(destroyTime);

        SoundManager.instance.PlaySE(logChange_sound);

        Instantiate(go_Log_prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 3f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 6f), Quaternion.LookRotation(go_ChildTree.transform.up));
        Instantiate(go_Log_prefabs, go_ChildTree.transform.position + (go_ChildTree.transform.up * 9f), Quaternion.LookRotation(go_ChildTree.transform.up));

        onLog.Invoke();

        Destroy(go_ChildTree.gameObject);
    }

    // 벌목 부분 자동 시선 처리
    public Vector3 GetTreeCenterPosition()
    {
        return go_treeCenter.transform.position;
    }
}
