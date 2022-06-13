using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// ����Ʈ�� ������ Ž��
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
        // FindAssets �Լ��� Asset �������� Filter�� �´� Asset�� GUID�� �������� �Լ�
        // GUID : Unity�� ������ �����ϱ� ���� ���������� ����ϴ� ID

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var quest = AssetDatabase.LoadAssetAtPath<T>(assetPath);

            if (quest.GetType() == typeof(T))
            {
                quests.Add(quest);
            }

            EditorUtility.SetDirty(this); // QuestDatabase ��ü�� ���� SerializeField ������ ��ȭ�� �������� ���� ����� �ݿ�
            AssetDatabase.SaveAssets();
        }
    }
#endif
}
