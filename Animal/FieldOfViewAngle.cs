using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField]
    private float viewAngle; // 시야각(120도)
    [SerializeField]
    private float viewDistance; // 시야거리(10미터)
    [SerializeField]
    private LayerMask targetMask; // 타겟 마스크(플레이어)

    private PlayerController thePlayer;
    private NavMeshAgent nav;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
    }

    // 플레이어 위치
    public Vector3 GetTargetPos()
    {
        return thePlayer.transform.position;
    }

    public bool View()
    {
        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance, targetMask);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if (_targetTf.name == "Player")
            {
                // 플레이어와 동물의 방향 정보(플레이어 위치 - 자신의 위치)
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                // 플레이어와 동물의 각도 정보
                float _angle = Vector3.Angle(_direction, transform.forward);

                // 설정 시야각의 반 각도 안에 있다면
                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;

                    // 동물에 충돌 레이저를 시야 거리 만큼 쏘아서 충돌 정보를 받아옴
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        // 충돌 레이저에 Player 정보를 받아오게 되면 true 반환
                        if (_hit.transform.name == "Player")
                        {
                            //Debug.Log("플레이어가 돼지 시야 내에 있습니다.");

                            // 유니티 상에서 파란색 레이저로 표시
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);

                            return true;
                        }
                    }
                }
            }

            // 플레이어가 동물의 사거리 안에서 달리기를 시전하면 true 반환
            if (thePlayer.GetRun())
            {
                if(CalcPathLength(thePlayer.transform.position) <= viewDistance)
                {
                    return true;
                }
            }

        }

        return false;
    }

    private float CalcPathLength(Vector3 _targetPos)
    {
        // nav 시스템 의해 계산된 경로 저장
        NavMeshPath _path = new NavMeshPath();
        // 플레이어 위치에서 동물 위치까지의 경로를 계산하여 저장
        nav.CalculatePath(_targetPos, _path);

        // vetor3 배열에 플레이어 위치, 동물 위치, 코너 2개 위치 저장
        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2];

        _wayPoint[0] = transform.position; // 동물 위치 저장
        _wayPoint[_path.corners.Length + 1] = _targetPos; // 마지막 경로 위치 저장(플레이어 위치)

        // 중간 코너 위치 저장
        float _pathLength = 0;
        for (int i = 0; i < _path.corners.Length; i++)
        {
            _wayPoint[i + 1] = _path.corners[i]; // 웨이포인트에 경로를 넣음
            _pathLength += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]); // 경로 길이 계산
        }

        return _pathLength;
    }
}
