using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 보상
[CreateAssetMenu(menuName = "Quest/Reward/Item", fileName = "ItemReward_")]
public class ItemReward : Reward
{
    public override void Give(Quest quest)
    {
        //GameSystem.Instance.AddScore(Quantity);
        //PlayerPrefs.SetInt("bonusScore", Quantity);
        //PlayerPrefs.Save();
    }
}
