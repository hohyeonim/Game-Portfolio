using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// event와 Trigger를 통해 QuestSystem에 보고
public class QuestReporter : MonoBehaviour
{
    [SerializeField]
    private Category category;
    [SerializeField]
    private TaskTarget target;
    [SerializeField]
    private int successCount;
    [SerializeField]
    private string[] colliderTags;


    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ReportIfPassCondition(collision);
    }

    // 보고
    public void Report()
    {
        QuestSystem.Instance.ReceiveReport(category, target, successCount);
    }

    // Trigger함수들의 Logic이 같으므로 따로 하나 더 만든 함수
    private void ReportIfPassCondition(Component other)
    {
        if (colliderTags.Any(x => other.CompareTag(x)))
            Report();
    }
}
