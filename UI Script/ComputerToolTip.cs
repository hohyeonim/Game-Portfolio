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

    // UI 활성화(kit 이름, 설명, 키트 생성 시 필요 재료, 개수)
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

    // UI 비활성화
    public void HideToolTip()
    {
        go_BaseUI.SetActive(false);
        kitName.text = "";
        kitDes.text = "";
        kitNeedItem.text = "";
    }

}
