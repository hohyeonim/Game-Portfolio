using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 업적 (업적 UI창)
public class AchievementView : MonoBehaviour
{
    [SerializeField]
    private RectTransform achievementGroup;
    [SerializeField]
    private AchievementDetailView achievementDetailViewPrefab;
    [SerializeField]
    private GameObject achievement_BaseUi;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        CreateDetailViews(questSystem.ActiveAchievements);
        CreateDetailViews(questSystem.CompletedAchievements);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!GameManager.isAchievementView)
            {
                CallAchievementView();
            }
            else
            {
                CloseAchievementView();
            }
        }
    }

    private void CallAchievementView()
    {
        GameManager.isAchievementView = true;
        achievement_BaseUi.SetActive(true);
    }

    private void CloseAchievementView()
    {
        GameManager.isAchievementView = false;
        achievement_BaseUi.SetActive(false);
    }

    private void CreateDetailViews(IReadOnlyList<Quest> achievements)
    {
        foreach (var achievement in achievements)
        {
            Instantiate(achievementDetailViewPrefab, achievementGroup).Setup(achievement);
        }
    }
}
