using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Task ����
public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    // UI ���� ������ UI Update Code�� Event�� ������ ������ ���°� �ٲ�� �˾Ƽ� UI�� Update ��
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category category;

    [Header("Text")]
    [SerializeField]
    private string codeName; // �˻��� ���� ��� ����� ���� ���������� ����ϴ� �̸�(Data ��)
    [SerializeField]
    private string description; // Task �̸�

    [Header("Action")]
    [SerializeField]
    private TaskAction action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue; // Task�� �ʱ� ���� ���� ����
    [SerializeField]
    private int needSuccessToComplete; // �ʿ��� ���� Ƚ��
    [SerializeField]
    private bool canReceiveReportsDuringCompletion; //����Ʈ�� �����߾ ��� ����Ƚ���� ���� ���� ���ΰ�?

    private TaskState state;
    private int currentSuccess;

    // Event Property
    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    // ���� ������ Ƚ��
    public int CurrentSuccess
    {
        get => currentSuccess;
        set
        {
            int prevSuccess = currentSuccess;
            currentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            if (currentSuccess != prevSuccess)
            {
                State = currentSuccess == needSuccessToComplete ? TaskState.Complete : TaskState.Running;
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess); // ?.�� �� ������ null�̸� null�� ��ȯ, �ƴϸ� �ڿ� �Լ��� ����
            }
        }
    }

    // property
    public Category Category => category;
    public string CodeName => codeName;
    public string Description => description;
    public int NeedSuccessToComplete => needSuccessToComplete;

    public TaskState State
    {
        get => state;
        set
        {
            var prevState = state;
            state = value;
            onStateChanged?.Invoke(this, state, prevState);
        }
    }
    public bool IsComplete => State == TaskState.Complete;
    
    // Task�� ���� Quest�� ��������
    public Quest Owner { get; private set; }

    // Awake ���� (owner ����)
    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    // Task ����
    public void Start()
    {
        State = TaskState.Running;
        if (initialSuccessValue)
            CurrentSuccess = initialSuccessValue.GetValue(this);
    }

    // Task ����
    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }

    // �ܺο��� ���� ������ Ƚ�� ���� ������ �� �ִ� �Լ�
    public void ReceiveReport(int successCount)
    {
        // ���� ����Ƚ���� ���� ����Ƚ���� �����ش�.
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    // Task ��ÿϷ�
    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }

    // TaskTarget�� ���� �� Task�� ���� Ƚ���� ���� ���� ������� Ȯ���ϴ� �Լ�
    // ���� �س��� Target�� Category�� �߿� �ش��ϴ� Target, Category���� ������ true, ������ false ��ȯ
    public bool IsTarget(string category, object target)
        => Category == category &&
        targets.Any(x => x.IsEqual(target)) &&
        (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion));

    // Ÿ���� �ִ��� Ȯ��
    public bool ContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
}
