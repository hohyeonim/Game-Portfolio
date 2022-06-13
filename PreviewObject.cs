using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public Building.Type needType;
    private bool needTypeFlag;

    // 충돌한 오브젝트의 collider
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField]
    private int layerGround; // 지상 레이어
    private const int IGNORE_RAYCAST_LAYER = 2;

    [SerializeField]
    private Material green;
    [SerializeField]
    private Material red;

    void Update()
    {
        ChangeColor();
    }

    // preview 색깔 변경
    private void ChangeColor()
    {
        if (needType == Building.Type.Normal) //needType이 건축 타입 Normal과 같다면
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

    // preview material 색 변경
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

    // 건축물 위에 건축을 할 수 있는 코드와 일반 건축을 할 수 있는 코드를 if문으로 구분
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            // 해당 건축물이 현재 설치하려고 하는 건축물의 needType과 다르면
            if (other.GetComponent<Building>().type != needType)
            {
                colliderList.Add(other); // 충돌 List 추가
            }
            else
            {
                needTypeFlag = true; // 설치 가능(건축물 위에 건축 가능)
            }
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Add(other); // 충돌 List 추가
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Structure")
        {
            if (other.GetComponent<Building>().type != needType)
            {
                colliderList.Remove(other); // 충돌 List 제거
            }
            else
            {
                needTypeFlag = false; // 설치 불가능(건축물 위에 건축 불가)
            }
        }
        else
        {
            if (other.gameObject.layer != layerGround && other.gameObject.layer != IGNORE_RAYCAST_LAYER)
            {
                colliderList.Remove(other); // 충돌 List 제거
            }
        }
    }

    public bool isBuildable()
    {
        // needType이 Normal타입과 같다면
        if (needType == Building.Type.Normal)
        {
            return colliderList.Count == 0; // 충돌 리스트가 0일 때 설치 가능
        }
        else
        {
            return colliderList.Count == 0 && needTypeFlag; // 충돌 리스트가 0이고 needTypeFlag가 true일 때 건축 가능
        }
    }
}
