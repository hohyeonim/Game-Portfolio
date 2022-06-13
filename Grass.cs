using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    // Ǯ ü��
    [SerializeField]
    private int hp;
    
    // ����Ʈ ���� �ð�
    [SerializeField]
    private float destroyTime;

    // ���߷� ����
    [SerializeField]
    private float force;

    // Ÿ�� ȿ��
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

    // ������ �ڵ� ȹ��
    private void Destruction()
    {
        // �κ��丮�� leaf ������ �ڵ� ȹ��
        theInven.AcquireItem(item_leaf, leafCount);

        // ������ rigidbody�� �ִ� �߷�, boxcollider�� Ȱ��ȭ, AddExplosionForce�� �̿��Ͽ� ���� and �𳯸����� Ǯ �������� �ڿ������� ������
        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].useGravity = true;
            rigidbodys[i].AddExplosionForce(force, transform.position, 1f);
            boxcolliders[i].enabled = true;
        }

        Destroy(this.gameObject, destroyTime);
    }
}
