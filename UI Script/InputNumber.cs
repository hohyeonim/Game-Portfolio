using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputNumber : MonoBehaviour
{
    private bool activated = false;

    [SerializeField]
    private Text text_Preview;
    [SerializeField]
    private Text text_Input;
    [SerializeField]
    private InputField if_text;

    [SerializeField]
    private GameObject go_Base;

    [SerializeField]
    private ActionController thePlayer;


    void Update()
    {
        if (activated)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OK();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cancel();
            }
        }
    }

    // ������ ������ â Ȱ��ȭ
    public void Call()
    {
        go_Base.SetActive(true);
        activated = true;
        if_text.text = "";
        text_Preview.text = DragSlot.instance.dragSlot.itemCount.ToString();
    }

    // ������ ������ â ���
    public void Cancel()
    {
        activated = false;
        go_Base.SetActive(false);
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    // ok��ư�� Ŭ���ϸ� text�� �ִ� ������ Ȯ���Ͽ� �ش� ������ ������ŭ ����
    public void OK()
    {
        DragSlot.instance.SetColor(0);

        int num;
        if (text_Input.text != "")
        {
            if (CheckNumber(text_Input.text))
            {
                num = int.Parse(text_Input.text);
                if (num > DragSlot.instance.dragSlot.itemCount)
                {
                    num = DragSlot.instance.dragSlot.itemCount;
                }
            }
            else
            {
                num = 1;
            }
        }
        else
        {
            num = int.Parse(text_Preview.text);
        }

        StartCoroutine(DropItemCoroutine(num));
    }

    IEnumerator DropItemCoroutine(int _num)
    {
        // ĳ���� ī�޶� ��ġ �տ� ���� ������ ����
        for (int i = 0; i < _num; i++)
        {
            if (DragSlot.instance.dragSlot.item.itemPrefab != null)
            {
                Instantiate(DragSlot.instance.dragSlot.item.itemPrefab, thePlayer.transform.position + thePlayer.transform.forward, Quaternion.identity);
            }
            
            DragSlot.instance.dragSlot.SetSlotCount(-1);
            yield return new WaitForSeconds(0.05f);
        }

        // ���� �������� ��� �ְ�, �� �������� ��� ������ ���� ��� ������ �ı�
        if (int.Parse(text_Preview.text) == _num)
        {
            if (QuickSlotController.go_HandItem != null)
            {
                Destroy(QuickSlotController.go_HandItem);
            }
        }

        DragSlot.instance.dragSlot = null;
        go_Base.SetActive(false);
        activated = false;
    }

    // �������� �ƴ��� üũ
    private bool CheckNumber(string _argString)
    {
        char[] _tempCharArry = _argString.ToCharArray();
        bool isNumber = true;
        for (int i = 0; i < _tempCharArry.Length; i++)
        {
            // �ƽ�Ű �ڵ�� ���� ���� ���(48~57)
            if (_tempCharArry[i] >= 48 && _tempCharArry[i] <= 57)
            {
                continue;
            }
            isNumber = false;
        }

        return isNumber;
    }
}
