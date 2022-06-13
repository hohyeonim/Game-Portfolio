using UnityEngine;
using System.Collections;

public class WeakAnimalSpawn : MonoBehaviour
{
    //������ ������ ��ġ�� ���� �迭
    public Transform[] points;
    //���� �������� �Ҵ��� ����
    public GameObject[] animalPrefabs;

    //���� �߻���ų �ֱ�
    public float createTime;
    //���� �ִ� �߻� ����
    public int maxMonster = 10;
    //���� ���� ���� ����
    public bool isGameOver = false;

    // Use this for initialization
    void Start()
    {
        //Hierarchy View�� Spawn Point�� ã�� ������ �ִ� ��� Transform ������Ʈ�� ã�ƿ�
        points = GameObject.Find("SpawnPoint1").GetComponentsInChildren<Transform>();

        if (points.Length > 0)
        {
            //���� ���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(this.CreateMonster());
        }
    }

    IEnumerator CreateMonster()
    {
        //���� ���� �ñ��� ���� ����
        while (!isGameOver)
        {
            //���� ������ ���� ���� ����
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("WeakAnimal").Length;

            if (monsterCount < maxMonster)
            {
                //���� ���� �ֱ� �ð���ŭ ���
                yield return new WaitForSeconds(createTime);

                //�ұ�Ģ���� ��ġ ����
                int idx = Random.Range(0, points.Length);
                int animal = Random.Range(0, animalPrefabs.Length);

                //���� ���� ����
                Instantiate(animalPrefabs[animal], points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
}