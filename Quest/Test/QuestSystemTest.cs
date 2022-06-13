using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystemTest : MonoBehaviour
{
    [SerializeField]
    private Quest quest;
    [SerializeField]
    private Category category;
    [SerializeField]
    private TaskTarget target;

    void Start()
    {
        var questSystem = QuestSystem.Instance;

        questSystem.onQuestRegistered += (quest) =>
        {
            print($"New Quest:{quest.CodeName} Registered");
            print($"Active Quest Count:{questSystem.ActiveQuests.Count}");
        };

        questSystem.onQuestCompleted += (quest) =>
        {
            print($"Quest:{quest.CodeName} Completed");
            print($"Completed Quest Count:{questSystem.CompletedQuests.Count}");
        };

        var newQuest = questSystem.Register(quest);
        newQuest.onTaskSuccessChanged += (quest, task, currentSuccess, prevSuccess) =>
        {
            print($"Quest:{quest.CodeName}, Task:{task.CodeName}, CurrentSuccess:{currentSuccess}");
        };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            QuestSystem.Instance.ReceiveReport(category, target, 1);
        }
    }
}
