using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class GDModifAttribute : Attribute
{
    public GDModifAttribute() { }
}