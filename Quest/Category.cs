using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Category", fileName = "Category_")]
public class Category : ScriptableObject, IEquatable<Category>
{
    [SerializeField]
    private string codeName;
    [SerializeField]
    private string displayName;

    // Property
    public string CodeName => codeName;
    public string DisplayName => displayName;

    // ī�װ��� ���ڿ��� �ٷ� ���� �� �ֵ��� �� ������ �ۼ�
    #region Operator
    public bool Equals(Category other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(other, this))
            return true;
        if (GetType() != other.GetType())
            return false;

        return codeName == other.CodeName;
    }

    public override int GetHashCode() => (CodeName, DisplayName).GetHashCode();

    public override bool Equals(object other) => base.Equals(other);

    // string���� �� ������
    public static bool operator ==(Category lhs, string rhs)
    {
        if (lhs is null)
            return ReferenceEquals(rhs, null);
        return lhs.CodeName == rhs || lhs.DisplayName == rhs;
    }

    // ��Ī ������
    public static bool operator !=(Category lhs, string rhs) => !(lhs == rhs);
    #endregion
}
