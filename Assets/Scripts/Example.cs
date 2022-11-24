using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField] private EnumDataContainer<TextStyle, TEXTSTYLES> _textStyles;

    [ExposedField]
    private int _playerMoney;

    [ExposedField]
    private int _playerHealth;
    
    [ExposedField]
    private int _playerMaxHealth;
    
    [ExposedField]
    private int _playerBullets;

    [ExposedField]
    private string _hey = "Salut";
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