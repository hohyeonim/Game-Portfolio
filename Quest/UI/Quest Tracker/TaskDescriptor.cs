using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Task의 정보를 출력
public class TaskDescriptor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    // 상황에 따른 Color 지정
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color taskCompletionColor;
    [SerializeField]
    private Color taskSuccessCountColor;
    [SerializeField]
    private Color strikeThroughColor;

    // 외부에서 사용할 Text Update
    public void UpdateText(string text)
    {
        this.text.fontStyle = FontStyles.Normal;
        this.text.text = text;
    }

    // 외부에서 사용할 Text Update
    public void UpdateText(Task task)
    {
        text.fontStyle = FontStyles.Normal;

        if (task.IsComplete)
        {
            var colorCode = ColorUtility.ToHtmlStringRGB(taskCompletionColor);
            text.text = BuildText(task, colorCode, colorCode);
        }
        else
            text.text = BuildText(task, ColorUtility.ToHtmlStringRGB(normalColor), ColorUtility.ToHtmlStringRGB(taskSuccessCountColor));
    }

    // 앞서 깨진 퀘스트는 착선 표시
    public void UpdateTextUsingStrikeThrough(Task task)
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(strikeThroughColor);
        text.fontStyle = FontStyles.Strikethrough;
        text.text = BuildText(task, colorCode, colorCode);
    }

    // 출력할 문장
    private string BuildText(Task task, string textColorCode, string successCountColorCode)
    {
        return $"<color=#{textColorCode}>● {task.Description} <color=#{successCountColorCode}>{task.CurrentSuccess}</color>/{task.NeedSuccessToComplete}</color>";
    }
}
