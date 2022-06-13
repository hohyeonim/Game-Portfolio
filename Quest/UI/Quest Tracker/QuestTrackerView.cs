using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

// QuestTracker를 제어
public class QuestTrackerView : MonoBehaviour
{
    [SerializeField]
    private QuestTracker questTrackerPrefab;
    [SerializeField]
    private CategoryColor[] categoryColors;

    // 초기 Tracker를 생성, QuestSystem에 event를 등록해서 새로운 Quest가 등록되면 QuestTracker가 생성
    private void Start()
    {
        QuestSystem.Instance.onQuestRegistered += CreateQuestTracker;

        foreach (var quest in QuestSystem.Instance.ActiveQuests)
            CreateQuestTracker(quest);
    }

    // QuestSystem에 event 해제
    private void OnDestroy()
    {
        if (QuestSystem.Instance)
            QuestSystem.Instance.onQuestRegistered -= CreateQuestTracker;
    }

    //QuestTracker를 생성
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