using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Task Group 상태
public enum TaskGroupState
{
    Inactive,
    Running,
    Complete
}

[System.Serializable]
public class TaskGroup
{
    [SerializeField]
    private Task[] tasks;

    // Property
    public IReadOnlyList<Task> Tasks => tasks;
    public Quest Owner { get; private set; } // Task Group 소유주
    public bool IsAllTaskComplete => tasks.All(x => x.IsComplete); // Task들이 다 완료 되었는지 확인해주는 Property
    public bool IsComplete => State == TaskGroupState.Complete; // 완료 확인
    public TaskGroupState State { get; private set; }

    public TaskGroup(TaskGroup copyTarget)
    {
        tasks = copyTarget.Tasks.Select(x => Object.Instantiate(x)).ToArray();
    }

    // 소유주 셋팅
    public void Setup(Quest owner)
    {
        Owner = owner;
        foreach (var task in tasks)
            task.Setup(owner);
    }

    // Task들 시작
    public void Start()
    {
        State = TaskGroupState.Running;
        foreach (var task in tasks)
            task.Start();
    }

    // Task들 종료
    public void End()
    {
        foreach (var task in tasks)
            task.End();
    }

    // Task에 성공 횟수 전달
    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach (var task in tasks)
        {
            if (task.IsTarget(category, target)) // Task가 해당 카테고리와 타겟을 가지고 있으면
                task.ReceiveReport(successCount); // 보고 받음
        }
    }

    // Task 완료 처리
    public void Complete()
    {
        if (IsComplete)
            return;

        State = TaskGroupState.Complete;

        foreach (var task in tasks)
        {
            if (!task.IsComplete)
                task.Complete();
        }
    }

    // Target을 가진 Task를 찾아오는 함수
    public Task FindTaskByTarget(object target) => tasks.FirstOrDefault(x => x.ContainsTarget(target));
    public Task FindTaskByTarget(TaskTarget target) => FindTaskByTarget(target.Value);

    // Target을 가진 Task가 있는지 확인하는 함수
    public bool ContainsTarget(object target) => tasks.Any(x => x.ContainsTarget(target));
    public bool ContainsTarget(TaskTarget target) => ContainsTarget(target.Value);
}
