using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Task 상태
public enum TaskState
{
    Inactive,
    Running,
    Complete
}

[CreateAssetMenu(menuName = "Quest/Task/Task", fileName = "Task_")]
public class Task : ScriptableObject
{
    // UI 같은 곳에서 UI Update Code를 Event에 연결해 놓으면 상태가 바뀌면 알아서 UI가 Update 됨
    #region Events
    public delegate void StateChangedHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    #endregion

    [SerializeField]
    private Category category;

    [Header("Text")]
    [SerializeField]
    private string codeName; // 검색과 같은 어떠한 기능을 위해 내부적으로 사용하는 이름(Data 비교)
    [SerializeField]
    private string description; // Task 이름

    [Header("Action")]
    [SerializeField]
    private TaskAction action;

    [Header("Target")]
    [SerializeField]
    private TaskTarget[] targets;

    [Header("Setting")]
    [SerializeField]
    private InitialSuccessValue initialSuccessValue; // Task의 초기 성공 값을 정함
    [SerializeField]
    private int needSuccessToComplete; // 필요한 성공 횟수
    [SerializeField]
    private bool canReceiveReportsDuringCompletion; //퀘스트가 성공했어도 계속 성공횟수를 보고 받을 것인가?

    private TaskState state;
    private int currentSuccess;

    // Event Property
    public event StateChangedHandler onStateChanged;
    public event SuccessChangedHandler onSuccessChanged;

    // 현재 성공한 횟수
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
                onSuccessChanged?.Invoke(this, currentSuccess, prevSuccess); // ?.은 이 변수가 null이면 null을 반환, 아니면 뒤에 함수를 실행
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
    
    // Task를 가진 Quest가 누구인지
    public Quest Owner { get; private set; }

    // Awake 역할 (owner 셋팅)
    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    // Task 시작
    public void Start()
    {
        State = TaskState.Running;
        if (initialSuccessValue)
            CurrentSuccess = initialSuccessValue.GetValue(this);
    }

    // Task 종료
    public void End()
    {
        onStateChanged = null;
        onSuccessChanged = null;
    }

    // 외부에서 현재 성공한 횟수 값을 조작할 수 있는 함수
    public void ReceiveReport(int successCount)
    {
        // 들어온 성공횟수를 현재 성공횟수에 더해준다.
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    // Task 즉시완료
    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }

    // TaskTarget을 통해 이 Task가 성공 횟수를 보고 받을 대상인지 확인하는 함수
    // 셋팅 해놓은 Target과 Category들 중에 해당하는 Target, Category들이 있으면 true, 없으면 false 반환
    public bool IsTarget(string category, object target)
        => Category == category &&
        targets.Any(x => x.IsEqual(target)) &&
        (!IsComplete || (IsComplete && canReceiveReportsDuringCompletion));

    // 타겟이 있는지 확인
    public bool ContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
}
