using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchemyToolTip : MonoBehaviour
{
    [SerializeField]
    private Text txt_NeedItemName;
    [SerializeField]
    private Text txt_NeedItemNumber;

    [SerializeField]
    private GameObject go_BaseToolTip;

    private void Clear()
    {
        txt_NeedItemName.text = "";
        txt_NeedItemNumber.text = "";
    }

    // tooltip UI 활성화
    public void ShowToolTip(string[] _needItemName, int[] _needItemNumber)
    {
        Clear();
        go_BaseToolTip.SetActive(true);

        for (int i = 0; i < _needItemNumber.Length; i++)
        {
            txt_NeedItemName.text += _needItemName[i] + "\n"; // 재료 이름
            txt_NeedItemNumber.text += "x " + _needItemNumber[i] + "\n"; // 재료 개수
        }
    }

    // tooltip UI 비활성화
    public void HideToolTip()
    {
        Clear();
        go_BaseToolTip.SetActive(false);
    }
}
