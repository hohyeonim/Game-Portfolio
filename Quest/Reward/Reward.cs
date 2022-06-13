using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 퀘스트 보상 스크립트
public abstract class Reward : ScriptableObject
{
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private string description;
    [SerializeField]
    private int quantity;

    // Property
    public Sprite Icon => icon;
    public string Description => description;
    public int Quantity => quantity;

    // 퀘스트 보상
    public abstract void Give(Quest quest);
}
