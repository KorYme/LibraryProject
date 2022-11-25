using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField] private EnumDataContainer<TextStyle, TEXTSTYLES> _textStyles;

    [ExposedField("PlayerMoney", "ExampleGameObject")]
    public int _playerMoney;

    [ExposedField("PlayerHealth", "ExampleGameObject")]
    private int _playerHealth;

    [ExposedField("PlayerMaxHealth", "ExampleGameObject")]
    private int _playerMaxHealth;

    [ExposedField("PlayerBullets", "ExampleGameObject")]
    private int _playerBullets;

    [ExposedField("Hey", "ExampleGameObject")]
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