using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    protected StatusController thePlayerStatus;

    [SerializeField]
    public string animalName; // ������ �̸�
    [SerializeField]
    protected int hp; // ������ ü��

    [SerializeField]
    private Item item_Prefab; // ������
    [SerializeField]
    public int itemNumber; // ������ ȹ�� ����

    [SerializeField]
    protected float walkSpeed; // �ȱ� ���ǵ�
    [SerializeField]
    protected float runSpeed; // �ٱ� ���ǵ�

    protected Vector3 destination; // ����

    // ���º���
    protected bool isAction; // �ൿ ������ �ƴ��� �Ǻ�
    protected bool isWalking; // �ȴ��� �� �ȴ��� �Ǻ�
    protected bool isRunning; // �ٴ��� �Ǻ�
    protected bool isChasing; // �߰� ������ �Ǻ�
    protected bool isAttacking; // ���� ������ �Ǻ�
    public bool isDead; // �׾����� �Ǻ�

    [SerializeField]
    protected float walkTime; // �ȱ� �ð�
    [SerializeField]
    protected float waitTime; // ��� �ð�
    [SerializeField]
    protected float runTime; // �ٱ� �ð�
    protected float currentTime;

    // �ʿ��� ������Ʈ
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    protected Rigidbody rigid;
    [SerializeField]
    protected BoxCollider boxCol;
    protected AudioSource theAudio;
    protected NavMeshAgent nav;
    protected FieldOfViewAngle theViewAngle;

    [SerializeField]
    protected AudioClip[] sound_Normal;
    [SerializeField]
    protected AudioClip sound_Hurt;
    [SerializeField]
    protected AudioClip sound_Dead;

    public UnityEngine.Events.UnityEvent onDead;

    void Start()
    {
        thePlayerStatus = FindObjectOfType<StatusController>();
        theViewAngle = GetComponent<FieldOfViewAngle>();
        nav = GetComponent<NavMeshAgent>();
        theAudio = GetComponent<AudioSource>();
        currentTime = waitTime;
        isAction = true;
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            Move();
            ElapseTime();
        }
    }

    protected void Move()
    {
        if (isWalking || isRunning)
        {
            //rigid.MovePosition(transform.position + (transform.forward * applySpeed * Time.deltaTime));
            nav.SetDestination(transform.position + destination * 5f);
        }
    }

    protected void ElapseTime()
    {
        if (isAction)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0 && !isChasing && !isAttacking)
            {
                ReSet();
            }
        }
    }

    protected virtual void ReSet()
    {
        isWalking = false;
        isRunning = false;
        isAction = true;
        nav.speed = walkSpeed;
        nav.ResetPath();
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        destination.Set(Random.Range(-0.2f, 0.2f), 0f, Random.Range(0.5f, 1f));
    }



    // �ȱ�
    protected void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        nav.speed = walkSpeed;
    }

    // �������� �Ծ��� ��
    public virtual void Damage(int _dmg, Vector3 _targetPos)
    {
        if (!isDead)
        {
            hp -= _dmg;

            if (hp <= 0)
            {
                Dead();
                onDead.Invoke();
                return;
            }

            PlaySE(sound_Hurt);
        }
    }

    // ������ �׾��� ��
    protected void Dead()
    {
        PlaySE(sound_Dead);
        isWalking = false;
        isRunning = false;
        isChasing = false;
        isAttacking = false;
        isDead = true;
        nav.ResetPath();
        anim.SetTrigger("Dead");
    }

    // ���� �����Ҹ� ����
    protected void RandomSound()
    {
        int _random = Random.Range(0, sound_Normal.Length);
        PlaySE(sound_Normal[_random]);
    }

    protected void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    public Item GetItem()
    {
        this.gameObject.tag = "Untagged";
        Destroy(this.gameObject, 3f);
        return item_Prefab;
    }
}