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
    private Transform tf_ItemAppear; // ������ ������ ��ġ
    [SerializeField]
    private GameObject go_BaseUI;

    private bool isCraft = false; // �ߺ� ���� ����
    public bool isPowerOn = false; // ����

    // �ʿ��� ������Ʈ
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

    // ��ǻ�� ���� ON
    public void PowerOn()
    {
        GameManager.isComputerKit = true;

        isPowerOn = true;
        go_BaseUI.SetActive(true);
    }

    // ��ǻ�� ���� OFF
    public void PowerOff()
    {
        GameManager.isComputerKit = false;

        isPowerOn = false;
        theTooltip.HideToolTip();
        go_BaseUI.SetActive(false);
    }

    // �ش� ������ ���� UI Ȱ��ȭ
    public void ShowToolTip(int _buttonNum)
    {
        theTooltip.ShowToolTip(kits[_buttonNum].kitName, kits[_buttonNum].kitDescription, kits[_buttonNum].needItemName, kits[_buttonNum].needItemNumber);
    }

    // �ش� ������ ���� UI ��Ȱ��ȭ
    public void HideToolTip()
    {
        theTooltip.HideToolTip();
    }

    private void PlaySE(AudioClip _clip)
    {
        theAudio.clip = _clip;
        theAudio.Play();
    }

    // ��ư Ŭ�� �� ��� üũ �� ŰƮ ����
    public void ClickButton(int _slotNumber)
    {
        PlaySE(sound_ButtonClick);

        if (!isCraft)
        {
            if (!CheckIngredient(_slotNumber)) // ��� üũ
            {
                return;
            }

            isCraft = true;

            UseIngredient(_slotNumber); // ��� ���

            StartCoroutine(CraftCoroutine(_slotNumber)); // ŰƮ ����
        }
    }

    // �κ��丮�� �ش� Kit�� �ʿ��� ��ᰡ �ִ��� �Ǻ�
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

    // kit�� �ʿ��� ��� �Ҹ�
    private void UseIngredient(int _slotNumber)
    {
        for (int i = 0; i < kits[_slotNumber].needItemName.Length; i++)
        {
            theInven.SetItemCount(kits[_slotNumber].needItemName[i], kits[_slotNumber].needItemNumber[i]);
        }
    }

    // ���� �ڷ�ƾ(���� ���� 3�� �� �ش� ��ġ���� kit ����)
    IEnumerator CraftCoroutine(int _slotNumber)
    {
        PlaySE(sound_Activated);

        yield return new WaitForSeconds(3f);

        PlaySE(sound_Output);

        Instantiate(kits[_slotNumber].go_kit_Prefab, tf_ItemAppear.position, Quaternion.identity);
        isCraft = false;
    }
}
