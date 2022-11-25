using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ExposedFieldAttribute : Attribute
{
    public string _displayName;
    public string _gameObjectName;

    public ExposedFieldAttribute(string displayName = null, string gameObjectName = "Others")
    {
        _displayName = displayName;
        if (gameObjectName == null)
        {
            _gameObjectName = "Others";
        }
        else
        {
            _gameObjectName = gameObjectName;
        }
    }
}

public interface IModifiable
{
    public Component instance { get; }
}