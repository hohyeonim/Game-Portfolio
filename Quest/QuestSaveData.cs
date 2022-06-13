// 퀘스트에 저장 Data를 담을 QuestData 구조체
public struct QuestSaveData
{
    public string codeName;
    public QuestState state;
    public int taskGroupIndex;
    public int[] taskSuccessCounts;
}
