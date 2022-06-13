using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Kit
{
    public string kitName;
    public string kitDescription;
    public string[] needItemName;
    public int[] needItemNumber;

    public GameObject go_kit_Prefab;
}

public class ComputerKit : MonoBehaviour
{
    [SerializeField]
    private Kit[] kits;

    [SerializeField]
    private Transform tf_ItemAppear; // 생성될 아이템 위치
    [SerializeField]
    private GameObject go_BaseUI;

    private bool isCraft = false; // 중복 실행 방지
    public bool isPowerOn = false; // 전원

    // 필요한 컴포넌트
    private Inventory theInven;
    [SerializeField]
    private ComputerToolTip theTooltip;

    private AudioSource theAudio;
    [SerializeField]
    private AudioClip sound_ButtonClick;
    [SerializeField]
    private AudioClip sound_Beep;
    [SerializeField]
    private AudioClip sound_Activated;
    [SerializeField]
    private AudioClip sound_Output;

    void Start()
    {
        theInven = FindObjectOfType<Inventory>();
        theAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPowerOn)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PowerOff();
            }
        }
    }

    // 컴퓨터 전원 ON
    public void PowerOn()
    {
        GameManager.isComputerKit = true;

        isPowerOn = true;
        go_BaseUI.SetActive(true);
    }

    // 컴퓨터 전원 OFF
    public void PowerOff()
    {
        GameManager.isComputerKit = false;

        isPowerOn = false;
        theTooltip.HideToolTip();
        go_BaseUI.SetActive(false);
    }

    // 해당 아이템 설명 UI 활성화
    public void ShowToolTip(int _buttonNum)
    {
        theTooltip.ShowToolTip(kits[_buttonNum].kitName, kits[_buttonNum].kitDescription, kits[_buttonNum].needItemName, kits[_buttonNum].needItemNumber);
    }

    // 해당 아이템 설명 UI 비활성화
    public void HideToolTip()
    {
        theTooltip.HideToolTip();
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    // 버튼 클릭 시 재료 체크 후 키트 생성
    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);

        if (!isCraft)
        {
            if (!CheckIngredient(_slotNumber)) // 재료 체크
            {
                return;
            }

            isCraft = true;

            UseIngredient(_slotNumber); // 재료 사용

            StartCoroutine(CraftCoroutine(_slotNumber)); // 키트 생성
        }
    }

    // 인벤토리에 해당 Kit에 필요한 재료가 있는지 판별
    private bool CheckIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
            if(theInven.GetItemCount(kits[_slotNumber].needItemName[i]) < kits[_slotNumber].needItemNumber[i])
            {
                PlaySE(sound_Beep);
                return false;
            }
        }

        return true;
    }

    // kit에 필요한 재료 소모
    private void UseIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
            theInven.SetItemCount(kits[_slotNumber].needItemName[i], kits[_slotNumber].needItemNumber[i]);
        }
    }

    // 생산 코루틴(생산 사운드 3초 후 해당 위치에서 kit 생성)
    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);

        yield return new WaitForSeconds(3f);

        PlaySE(sound_Output);

        Instantiate(kits[_slotNumber].go_kit_Prefab, tf_ItemAppear.position, Quaternion.identity);
        isCraft = false;
    }
}
