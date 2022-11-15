using System;
using UnityEngine;

[Serializable]
public class EnumDataContainer<TValue, TEnum> where TEnum : Enum
{
    [SerializeField] private TValue[] _content = default(TValue[]);
    [SerializeField] private TEnum _enumType;

    public TValue this[int index]
    {
        get { return _content[index]; }
    }

    public int Length
    {
        get { return _content.Length; }
    }
}
