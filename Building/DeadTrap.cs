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

    // 활성화 확인 함수(재설치를 위해 넘겨주는 함수)
    public bool GetIsActivated()
    {
        return isActivated;
    }

    // 함정 재설치
    public void ReInstall()
    {
        isActivated = false;
        anim.SetTrigger("DeActivate");
    }

    // collider에 부딪히면 함정 발동
    private void OnTriggerEnter(Collider other)
    {
        if (!isActivated)
        {
            if (other.transform.tag != "Untagged" && other.transform.tag != "Trap")
            {
                StartCoroutine(theTrapDamage.ActivatedTrapCoroutine()); // 함정 발동 코루틴 실행
                isActivated = true;
                anim.SetTrigger("Activate");
                theAudio.clip = sound_Activate;
                theAudio.Play();
            }
        }
    }
}
