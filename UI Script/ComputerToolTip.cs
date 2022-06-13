using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComputerToolTip : MonoBehaviour
{
    [SerializeField]
    private GameObject go_BaseUI;

    [SerializeField]
    private Text kitName;
    [SerializeField]
    private Text kitDes;
    [SerializeField]
    private Text kitNeedItem;

    // UI Ȱ��ȭ(kit �̸�, ����, ŰƮ ���� �� �ʿ� ���, ����)
    public void ShowToolTip(string _kitName, string _kitDes, string[] _needItem, int[] _needItemNumber)
    {
        go_BaseUI.SetActive(true);

        kitName.text = _kitName;
        kitDes.text = _kitDes;

        for (int i = 0; i < _needItem.Length; i++)
        {
            kitNeedItem.text += _needItem[i];
            kitNeedItem.text += " x " + _needItemNumber[i].ToString() + "\n";
        }
    }

    // UI ��Ȱ��ȭ
    public void HideToolTip()
    {
        go_BaseUI.SetActive(false);
        kitName.text = "";
        kitDes.text = "";
        kitNeedItem.text = "";
    }

}
