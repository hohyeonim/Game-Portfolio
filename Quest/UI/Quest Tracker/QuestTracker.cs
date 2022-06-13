using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Quest의 정보 출력과 TaskDescriptor의 제어를 담당
public class QuestTracker : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI questTitleText;
    [SerializeField]
    private TaskDescriptor taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> taskDesriptorsByTask = new Dictionary<Task, TaskDescriptor>();

    private Quest targetQuest;

    // 등록한 event를 제거
    private void OnDestroy()
    {
        if (targetQuest != null)
        {
            targetQuest.onNewTaskGroup -= UpdateTaskDescriptos;
            targetQuest.onCompleted -= DestroySelf;
        }

        foreach (var tuple in taskDesriptorsByTask)
        {
            var task = tuple.Key;
            task.onSuccessChanged -= UpdateText;
        }
    }

    // Tracker를 Setup
    public void Setup(Quest targetQuest, Color titleColor)
    {
        this.targetQuest = targetQuest;

        questTitleText.text = targetQuest.Category == null ?
            targetQuest.DisplayName :
            $"[{targetQuest.Category.DisplayName}] {targetQuest.DisplayName}";

        questTitleText.color = titleColor;

        targetQuest.onNewTaskGroup += UpdateTaskDescriptos;
        targetQuest.onCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptos(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for (int i = 1; i < taskGroups.Count; i++)
            {
                var taskGroup = taskGroups[i];
                UpdateTaskDescriptos(targetQuest, taskGroup, taskGroups[i - 1]);

                if (taskGroup == targetQuest.CurrentTaskGroup)
                    break;
            }
        }
    }

    // Quest의 onNewTaskGroup event에 등록해서 TaskDescriptor 자체를 Update하는 함수
    private void UpdateTaskDescriptos(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (var task in currentTaskGroup.Tasks)
        {
            var taskDesriptor = Instantiate(taskDescriptorPrefab, transform);
            taskDesriptor.UpdateText(task);
            task.onSuccessChanged += UpdateText;

            taskDesriptorsByTask.Add(task, taskDesriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (var task in prevTaskGroup.Tasks)
            {
                var taskDesriptor = taskDesriptorsByTask[task];
                taskDesriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }

    // Text를 Update할 Callback 함수
    private void UpdateText(Task task, int currentSucess, int prevSuccess)
    {
        taskDesriptorsByTask[task].UpdateText(task);
    }

    // Tracker를 제거해줄 Callbaxk 함수
    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }
}
