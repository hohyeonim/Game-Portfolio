using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

// 화면 상단에 퀘스트가 완료됨을 알려줌(게임 종료 시)
public class QuestCompletionNotifier : MonoBehaviour
{
    [SerializeField]
    private string titleDescription;

    [SerializeField]
    private TextMeshProUGUI titleText;
    [SerializeField]
    private TextMeshProUGUI rewardText;
    [SerializeField]
    private float showTime = 3f;

    private Queue<Quest> reservedQuests = new Queue<Quest>();
    private StringBuilder stringBuilder = new StringBuilder();

    // event 시작
    private void Start()
    {
        var questSystem = QuestSystem.Instance;
        questSystem.onQuestCompleted += Notify;
        questSystem.onAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    // event 해제
    private void OnDestroy()
    {
        var questSysem = QuestSystem.Instance;
        if (questSysem != null)
        {
            questSysem.onQuestCompleted -= Notify;
            questSysem.onAchievementCompleted -= Notify;
        }
    }

    // 완료된 Quest를 등록
    private void Notify(Quest quest)
    {
        reservedQuests.Enqueue(quest);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine("ShowNotice");
        }
    }

    // Clear한 Quest의 정보를 보여줌
    private IEnumerator ShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        while (reservedQuests.Count > 0)
        {
            var quest = reservedQuests.Dequeue();

            titleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);

            foreach (var reward in quest.Rewards)
            {
                stringBuilder.Append(reward.Description);
                stringBuilder.Append(" ");
                stringBuilder.Append(reward.Quantity);
                stringBuilder.Append(" ");
            }

            rewardText.text = stringBuilder.ToString();
            stringBuilder.Clear();

            yield return waitSeconds;

        }

        gameObject.SetActive(false);
    }
}
