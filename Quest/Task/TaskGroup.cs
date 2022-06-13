using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Task Group ����
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
    public Quest Owner { get; private set; } // Task Group ������
    public bool IsAllTaskComplete => tasks.All(x => x.IsComplete); // Task���� �� �Ϸ� �Ǿ����� Ȯ�����ִ� Property
    public bool IsComplete => State == TaskGroupState.Complete; // �Ϸ� Ȯ��
    public TaskGroupState State { get; private set; }

    public TaskGroup(TaskGroup copyTarget)
    {
        tasks = copyTarget.Tasks.Select(x => Object.Instantiate(x)).ToArray();
    }

    // ������ ����
    public void Setup(Quest owner)
    {
        Owner = owner;
        foreach (var task in tasks)
            task.Setup(owner);
    }

    // Task�� ����
    public void Start()
    {
        State = TaskGroupState.Running;
        foreach (var task in tasks)
            task.Start();
    }

    // Task�� ����
    public void End()
    {
        foreach (var task in tasks)
            task.End();
    }

    // Task�� ���� Ƚ�� ����
    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach (var task in tasks)
        {
            if (task.IsTarget(category, target)) // Task�� �ش� ī�װ��� Ÿ���� ������ ������
                task.ReceiveReport(successCount); // ���� ����
        }
    }

    // Task �Ϸ� ó��
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

    // Target�� ���� Task�� ã�ƿ��� �Լ�
    public Task FindTaskByTarget(object target) => tasks.FirstOrDefault(x => x.ContainsTarget(target));
    public Task FindTaskByTarget(TaskTarget target) => FindTaskByTarget(target.Value);

    // Target�� ���� Task�� �ִ��� Ȯ���ϴ� �Լ�
    public bool ContainsTarget(object target) => tasks.Any(x => x.ContainsTarget(target));
    public bool ContainsTarget(TaskTarget target) => ContainsTarget(target.Value);
}
