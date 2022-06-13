using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeComponent : MonoBehaviour
{
    // ���� ���� ������
    [SerializeField]
    private GameObject[] go_treePieces;
    [SerializeField]
    private GameObject go_treeCenter;

    // �볪��
    [SerializeField]
    private GameObject go_Log_prefabs;

    // ������ ���� �������� ������ ���� ����
    [SerializeField]
    private float force;

    // �ڽ� Ʈ��
    [SerializeField]
    private GameObject go_ChildTree;

    // ����
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    // ���� ���� �ð�
    [SerializeField]
    private float debrisDestryTime;

    // ���� ���� �ð�
    [SerializeField]
    private float destroyTime;

    // �θ� Ʈ���� �ı��Ǹ� ĸ�� �ݶ��̴� ����
    [SerializeField]
    private CapsuleCollider parentCol;

    // �ڽ� Ʈ���� ������ �� �ʿ��� ������Ʈ Ȱ��ȭ �� �߷� Ȱ��ȭ
    [SerializeField]
    private CapsuleCollider childCol;
    [SerializeField]
    private Rigidbody childRigid;

    // �ʿ��� ����
    [SerializeField]
    private string chop_sound;
    [SerializeField]
    private string falldown_sound;
    [SerializeField]
    private string logChange_sound;

    // ����Ʈ �̺�Ʈ �ý���
    public UnityEngine.Events.UnityEvent onLog;

    // ���� main �Լ�
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

    // ���� ����Ʈ
    private void Hit(Vector3 _pos)
    {
        SoundManager.instance.PlaySE(chop_sound);

        // ���� ���� prefab ���� �� ����
        GameObject clone = Instantiate(go_hit_effect_prefab, _pos, Quaternion.Euler(Vector3.zero));
        Destroy(clone, debrisDestryTime);
    }

    // ������ ���� ���� ���� ����
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

    // ���� ���� ����
    private void DestroyPiece(int _num)
    {
        if (go_treePieces[_num].gameObject != null)
        {
            GameObject clone = Instantiate(go_hit_effect_prefab, go_treePieces[_num].transform.position, Quaternion.Euler(Vector3.zero));
            Destroy(clone, debrisDestryTime);
            Destroy(go_treePieces[_num].gameObject);
        }

    }

    // ���� �ʿ��� ���� ������ üũ
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

    // ���� ������ ����
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

    // �ڷ�ƾ : ������ �������鼭 ���� ���κ��� ������� �볪�� prefabs 3�� ����
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

    // ���� �κ� �ڵ� �ü� ó��
    public Vector3 GetTreeCenterPosition()
    {
        return go_treeCenter.transform.position;
    }
}
