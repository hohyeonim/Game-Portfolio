using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // 풀 체력
    [SerializeField]
    private int hp;
    
    // 이펙트 삭제 시간
    [SerializeField]
    private float destroyTime;

    // 폭발력 세기
    [SerializeField]
    private float force;

    // 타격 효과
    [SerializeField]
    private GameObject go_hit_effect_prefab;

    [SerializeField]
    private Item item_leaf;
    [SerializeField]
    private int leafCount;
    private Inventory theInven;

    private Rigidbody[] rigidbodys;
    private BoxCollider[] boxcolliders;

    [SerializeField]
    private string hit_sound;

    [SerializeField]
    private UnityEngine.Events.UnityEvent onGrass;

    void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        rigidbodys = this.transform.GetComponentsInChildren<Rigidbody>();
        boxcolliders = transform.GetComponentsInChildren<BoxCollider>();
    }

    public void Damage()
    {
        hp--;

        Hit();

        if (hp <= 0)
        {
            Destruction();
        }
    }

    private void Hit()
    {
        SoundManager.instance.PlaySE(hit_sound);

        var clone = Instantiate(go_hit_effect_prefab, transform.position + Vector3.up, Quaternion.identity);

        onGrass.Invoke();

        Destroy(clone, destroyTime);
    }

    // 나뭇잎 자동 획득
    private void Destruction()
    {
        // 인벤토리에 leaf 아이템 자동 획득
        theInven.AcquireItem(item_leaf, leafCount);

        // 나뭇잎 rigidbody에 있는 중력, boxcollider를 활성화, AddExplosionForce을 이용하여 폭발 and 흩날림으로 풀 조각들이 자연스럽게 떨어짐
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
            boxcolliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}
