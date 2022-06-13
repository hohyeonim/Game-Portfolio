using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

// QuestTracker�� ����
public class QuestTrackerView : MonoBehaviour
{
    [SerializeField]
    private QuestTracker questTrackerPrefab;
    [SerializeField]
    private CategoryColor[] categoryColors;

    // �ʱ� Tracker�� ����, QuestSystem�� event�� ����ؼ� ���ο� Quest�� ��ϵǸ� QuestTracker�� ����
    private void Start()
    {
        QuestSystem.Instance.onQuestRegistered += CreateQuestTracker;

        foreach (var quest in QuestSystem.Instance.ActiveQuests)
            CreateQuestTracker(quest);
    }

    // QuestSystem�� event ����
    private void OnDestroy()
    {
        if (QuestSystem.Instance)
            QuestSystem.Instance.onQuestRegistered -= CreateQuestTracker;
    }

    //QuestTracker�� ����
    private void CreateQuestTracker(Quest quest)
    {
        var categoryColor = categoryColors.FirstOrDefault(x => x.category == quest.Category);
        var color = categoryColor.category == null ? Color.white : categoryColor.color;
        Instantiate(questTrackerPrefab, transform).Setup(quest, color);
    }

    [System.Serializable]
    private struct CategoryColor
    {
        public Category category;
        public Color color;
    }
}