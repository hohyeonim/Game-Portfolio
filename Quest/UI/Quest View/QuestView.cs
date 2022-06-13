using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UI를 연결(Quest 창)
public class QuestView : MonoBehaviour
{
    [SerializeField]
    private QuestListViewController questListViewController;
    [SerializeField]
    private QuestDetailView questDetailView;
    [SerializeField]
    private GameObject quest_BaseUi;

    private void Start()
    {
        var questSystem = QuestSystem.Instance;

        foreach (var quest in questSystem.ActiveQuests)
            AddQuestToActiveListView(quest);

        foreach (var quest in questSystem.CompletedQuests)
            AddQuestToCompletedListView(quest);

        questSystem.onQuestRegistered += AddQuestToActiveListView;
        questSystem.onQuestCompleted += RemoveQuestFromActiveListView;
        questSystem.onQuestCompleted += AddQuestToCompletedListView;
        questSystem.onQuestCompleted += HideDetailIfQuestCanceled;
        questSystem.onQuestCanceled += HideDetailIfQuestCanceled;
        questSystem.onQuestCanceled += RemoveQuestFromActiveListView;

        foreach (var tab in questListViewController.Tabs)
            tab.onValueChanged.AddListener(HideDetail);
    }

    private void OnDestroy()
    {
        var questSystem = QuestSystem.Instance;
        if (questSystem)
        {
            questSystem.onQuestRegistered -= AddQuestToActiveListView;
            questSystem.onQuestCompleted -= RemoveQuestFromActiveListView;
            questSystem.onQuestCompleted -= AddQuestToCompletedListView;
            questSystem.onQuestCompleted -= HideDetailIfQuestCanceled;
            questSystem.onQuestCanceled -= HideDetailIfQuestCanceled;
            questSystem.onQuestCanceled -= RemoveQuestFromActiveListView;
        }
    }

    private void OnEnable()
    {
        if (questDetailView.Target != null)
            questDetailView.Show(questDetailView.Target);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (!GameManager.isQuestView)
            {
                CallQuestView();
            }
            else
            {
                CloseQuestView();
            }
        }
    }

    private void CallQuestView()
    {
        GameManager.isQuestView = true;
        quest_BaseUi.SetActive(true);
    }

    private void CloseQuestView()
    {
        GameManager.isQuestView = false;
        quest_BaseUi.SetActive(false);
    }


    private void ShowDetail(bool isOn, Quest quest)
    {
        if (isOn)
            questDetailView.Show(quest);
    }

    private void HideDetail(bool isOn)
    {
        questDetailView.Hide();
    }

    // 퀘스트들을 List View에 추가
    private void AddQuestToActiveListView(Quest quest)
        => questListViewController.AddQuestToActiveListView(quest, isOn => ShowDetail(isOn, quest));

    private void AddQuestToCompletedListView(Quest quest)
        => questListViewController.AddQuestToCompletedListView(quest, isOn => ShowDetail(isOn, quest));

    private void HideDetailIfQuestCanceled(Quest quest)
    {
        if (questDetailView.Target == quest)
            questDetailView.Hide();
    }

    private void RemoveQuestFromActiveListView(Quest quest)
    {
        questListViewController.RemoveQuestFromActiveListView(quest);
        if (questDetailView.Target == quest)
            questDetailView.Hide();
    }
}
