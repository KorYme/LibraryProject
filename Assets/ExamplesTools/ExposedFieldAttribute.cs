using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class ExposedFieldAttribute : Attribute
{
    public string _displayName;
    

    public ExposedFieldAttribute(string displayName = null)
    {
        _displayName = displayName;
    }
}
