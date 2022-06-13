using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// 퀘스트의 보관과 탐색
[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
    [SerializeField]
    private List<Quest> quests;

    public IReadOnlyList<Quest> Quests => quests;

    public Quest FindQuestBy(string codeName) => Quests.FirstOrDefault(x => x.CodeName == codeName);

#if UNITY_EDITOR
    [ContextMenu("FindQuests")]
    private void FindQuests()
    {
        FindQuestsBy<Quest>();
    }

    [ContextMenu("FindAchievements")]
    private void FindAchievements()
    {
        FindQuestsBy<Achievement>();
    }

    private void FindQuestsBy<T>() where T : Quest
    {
        quests = new List<Quest>();

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
        // FindAssets 함수는 Asset 폴더에서 Filter에 맞는 Asset의 GUID를 가져오는 함수
        // GUID : Unity가 에셋을 관리하기 위해 내부적으로 사용하는 ID

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
            {
                quests.Add(quest);
            }

            EditorUtility.SetDirty(this); // QuestDatabase 객체가 가진 SerializeField 변수가 변화가 생겼으니 에셋 저장시 반영
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
