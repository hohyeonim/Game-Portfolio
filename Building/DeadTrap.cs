using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrap : MonoBehaviour
{
    private Animator anim;
    private AudioSource theAudio;

    private bool isActivated = false;

    [SerializeField]
    private AudioClip sound_Activate;
    [SerializeField]
    private TrapDamage theTrapDamage;

    void Start()
    {
        anim = GetComponent<Animator>();
        theAudio = GetComponent<AudioSource>();
    }

    // Ȱ��ȭ Ȯ�� �Լ�(�缳ġ�� ���� �Ѱ��ִ� �Լ�)
    public bool GetIsActivated()
    {
        return isActivated;
    }

    // ���� �缳ġ
    public void ReInstall()
    {
        isActivated = false;
        anim.SetTrigger("DeActivate");
    }

    // collider�� �ε����� ���� �ߵ�
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            if (other.transform.tag != "Untagged" && other.transform.tag != "Trap")
            {
                StartCoroutine(theTrapDamage.ActivatedTrapCoroutine()); // ���� �ߵ� �ڷ�ƾ ����
                isActivated = true;
                anim.SetTrigger("Activate");
                theAudio.clip = sound_Activate;
                theAudio.Play();
            }
        }
    }
}
