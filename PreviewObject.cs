using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    // �浹�� ������Ʈ�� collider
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField]
    private int layerGround; // ���� ���̾�
    private const int IGNORE_RAYCAST_LAYER = 2;

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;

    void Update()
    {
        ChangeColor();
    }

    // preview ���� ����
    private void ChangeColor()
    {
        if (needType == Building.Type.Normal) //needType�� ���� Ÿ�� Normal�� ���ٸ�
        {
            if (colliderList.Count > 0) // 
            {
                SetColor(red);
            }
            else
            {
                SetColor(green);
            }
        }
        else
        {
            if (colliderList.Count > 0 || !needTypeFlag)
            {
                SetColor(red);
            }
            else
            {
                SetColor(green);
            }
        }
    }

    // preview material �� ����
    private void SetColor(Material mat)
    {
        foreach (Transform tf_Child in this.transform)
        {
            var newMaterial = new Material[tf_Child.GetComponent<Renderer>().materials.Length];

            for (int i = 0; i < newMaterial.Length; i++)
            {
                newMaterial[i] = mat;
            }

            tf_Child.GetComponent<Renderer>().materials = newMaterial;
        }
    }

    // ���๰ ���� ������ �� �� �ִ� �ڵ�� �Ϲ� ������ �� �� �ִ� �ڵ带 if������ ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            // �ش� ���๰�� ���� ��ġ�Ϸ��� �ϴ� ���๰�� needType�� �ٸ���
            if (other.GetComponent<Building>().type != needType)
            {
                colliderList.Add(other); // �浹 List �߰�
            }
            else
            {
                needTypeFlag = true; // ��ġ ����(���๰ ���� ���� ����)
            }
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Add(other); // �浹 List �߰�
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
            {
                colliderList.Remove(other); // �浹 List ����
            }
            else
            {
                needTypeFlag = false; // ��ġ �Ұ���(���๰ ���� ���� �Ұ�)
            }
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Remove(other); // �浹 List ����
            }
        }
    }

    public bool isBuildable()
    {
        // needType�� NormalŸ�԰� ���ٸ�
        if (needType == Building.Type.Normal)
        {
            return colliderList.Count == 0; // �浹 ����Ʈ�� 0�� �� ��ġ ����
        }
        else
        {
            return colliderList.Count == 0 && needTypeFlag; // �浹 ����Ʈ�� 0�̰� needTypeFlag�� true�� �� ���� ����
        }
    }
}
