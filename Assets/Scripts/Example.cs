using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField] private EnumDataContainer<TextStyle, TEXTSTYLES>[] _textStyles;
}

public enum TEXTSTYLES
{
    NORMAL,
    HEADING,
    WARNING,
    ERROR,
}

[System.Serializable]
public class TextStyle
{
    public int _size;
    public Color _color;
}