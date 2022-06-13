using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����Ʈ ���� ��ũ��Ʈ
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

    // ����Ʈ ����
    public abstract void Give(Quest quest);
}
