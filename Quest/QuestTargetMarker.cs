using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 가야할 위치 표시 및 타겟 위치 표시
public class QuestTargetMarker : MonoBehaviour
{
    [SerializeField]
    private TaskTarget target;
    [SerializeField]
    private MarkerMaterialData[] markerMaterialDatas;

    private Dictionary<Quest, Task> targetTasksByQuest = new Dictionary<Quest, Task>();
    private Transform cameraTransform;
    private Renderer renderer;

    private int currentRunningTargetTaskCount;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);

        QuestSystem.Instance.onQuestRegistered += TryAddTargetQuest;
        foreach (var quest in QuestSystem.Instance.ActiveQuests)
            TryAddTargetQuest(quest);
    }

    // Marker가 유저를 바라봄
    private void Update()
    {
        var rotation = Quaternion.LookRotation((cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180f, 0f);
    }

    // event 해제
    private void OnDestroy()
    {
        QuestSystem.Instance.onQuestRegistered -= TryAddTargetQuest;

        foreach (var keyPair in targetTasksByQuest)
        {
            var quest = keyPair.Key;
            var task = keyPair.Value;

            quest.onNewTaskGroup -= UpdateTargetTask;
            quest.onCompleted -= RemoveTargetQuest;
            task.onStateChanged -= UpdateRunningTargetTaskCount;
        }
    }
     
    // 등록된 Quest를 확인하여 Target일 경우 감시
    private void TryAddTargetQuest(Quest quest)
    {
        if (target != null && quest.ContainsTarget(target))
        {
            quest.onNewTaskGroup += UpdateTargetTask;
            quest.onCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }

    // 감시 중인 Task 교체
    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        targetTasksByQuest.Remove(quest);

        var task = currentTaskGroup.FindTaskByTarget(target);
        if (task != null)
        {
            targetTasksByQuest[quest] = task;
            task.onStateChanged += UpdateRunningTargetTaskCount;

            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    // Quest 완료시 Target에서 지워줌
    private void RemoveTargetQuest(Quest quest) => targetTasksByQuest.Remove(quest);

    // Task의 상태에 따라 Count 조절, Count가 0이면 Marker(Off), 0 이상이면 Marker(On)
    private void UpdateRunningTargetTaskCount(Task task, TaskState currentState, TaskState prevState = TaskState.Inactive)
    {
        if (currentState == TaskState.Running)
        {
            renderer.material = markerMaterialDatas.First(x => x.category == task.Category).markerMaterial;
            currentRunningTargetTaskCount++;
        }
        else
            currentRunningTargetTaskCount--;

        gameObject.SetActive(currentRunningTargetTaskCount != 0);
    }

    // Material 교체 방식으로 카테고리에 따라 다른 image의 Marker를 보여줌(Pair 구조체)
    [System.Serializable]
    private struct MarkerMaterialData
    {
        public Category category;
        public Material markerMaterial;
    }
}

