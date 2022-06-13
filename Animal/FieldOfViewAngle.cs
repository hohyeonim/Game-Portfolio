using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewAngle : MonoBehaviour
{
    [SerializeField]
    private float viewAngle; // �þ߰�(120��)
    [SerializeField]
    private float viewDistance; // �þ߰Ÿ�(10����)
    [SerializeField]
    private LayerMask targetMask; // Ÿ�� ����ũ(�÷��̾�)

    private PlayerController thePlayer;
    private NavMeshAgent nav;

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerController>();
        nav = GetComponent<NavMeshAgent>();
    }

    // �÷��̾� ��ġ
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
                // �÷��̾�� ������ ���� ����(�÷��̾� ��ġ - �ڽ��� ��ġ)
                Vector3 _direction = (_targetTf.position - transform.position).normalized;
                // �÷��̾�� ������ ���� ����
                float _angle = Vector3.Angle(_direction, transform.forward);

                // ���� �þ߰��� �� ���� �ȿ� �ִٸ�
                if (_angle < viewAngle * 0.5f)
                {
                    RaycastHit _hit;

                    // ������ �浹 �������� �þ� �Ÿ� ��ŭ ��Ƽ� �浹 ������ �޾ƿ�
                    if (Physics.Raycast(transform.position + transform.up, _direction, out _hit, viewDistance))
                    {
                        // �浹 �������� Player ������ �޾ƿ��� �Ǹ� true ��ȯ
                        if (_hit.transform.name == "Player")
                        {
                            //Debug.Log("�÷��̾ ���� �þ� ���� �ֽ��ϴ�.");

                            // ����Ƽ �󿡼� �Ķ��� �������� ǥ��
                            Debug.DrawRay(transform.position + transform.up, _direction, Color.blue);

                            return true;
                        }
                    }
                }
            }

            // �÷��̾ ������ ��Ÿ� �ȿ��� �޸��⸦ �����ϸ� true ��ȯ
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
        // nav �ý��� ���� ���� ��� ����
        NavMeshPath _path = new NavMeshPath();
        // �÷��̾� ��ġ���� ���� ��ġ������ ��θ� ����Ͽ� ����
        nav.CalculatePath(_targetPos, _path);

        // vetor3 �迭�� �÷��̾� ��ġ, ���� ��ġ, �ڳ� 2�� ��ġ ����
        Vector3[] _wayPoint = new Vector3[_path.corners.Length + 2];

        _wayPoint[0] = transform.position; // ���� ��ġ ����
        _wayPoint[_path.corners.Length + 1] = _targetPos; // ������ ��� ��ġ ����(�÷��̾� ��ġ)

        // �߰� �ڳ� ��ġ ����
        float _pathLength = 0;
        for (int i = 0; i < _path.corners.Length; i++)
        {
            _wayPoint[i + 1] = _path.corners[i]; // ��������Ʈ�� ��θ� ����
            _pathLength += Vector3.Distance(_wayPoint[i], _wayPoint[i + 1]); // ��� ���� ���
        }

        return _pathLength;
    }
}
