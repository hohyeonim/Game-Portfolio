using UnityEngine;
using System.Collections;

public class WeakAnimalSpawn : MonoBehaviour
{
    //동물이 출현할 위치를 담을 배열
    public Transform[] points;
    //동물 프리팹을 할당할 변수
    public GameObject[] animalPrefabs;

    //동물 발생시킬 주기
    public float createTime;
    //동물 최대 발생 개수
    public int maxMonster = 10;
    //게임 종료 여부 변수
    public bool isGameOver = false;

    // Use this for initialization
    void Start()
    {
        //Hierarchy View의 Spawn Point를 찾아 하위에 있는 모든 Transform 컴포넌트를 찾아옴
        points = GameObject.Find("SpawnPoint1").GetComponentsInChildren<Transform>();

        if (points.Length > 0)
        {
            //동물 생성 코루틴 함수 호출
            StartCoroutine(this.CreateMonster());
        }
    }

    IEnumerator CreateMonster()
    {
        //게임 종료 시까지 무한 루프
        while (!isGameOver)
        {
            //현재 생성된 동물 개수 산출
            int monsterCount = (int)GameObject.FindGameObjectsWithTag("WeakAnimal").Length;

            if (monsterCount < maxMonster)
            {
                //동물 생성 주기 시간만큼 대기
                yield return new WaitForSeconds(createTime);

                //불규칙적인 위치 산출
                int idx = Random.Range(0, points.Length);
                int animal = Random.Range(0, animalPrefabs.Length);

                //동물 동적 생성
                Instantiate(animalPrefabs[animal], points[idx].position, points[idx].rotation);
            }
            else
            {
                yield return null;
            }
        }
    }
}