using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace ToolLibrary
{
    [AttributeUsage(AttributeTargets.Field)]
    public class GDModifAttribute : Attribute
    {

        public bool _isASceneObject;

        public GDModifAttribute(bool isASceneObject = true)
        {
            _isASceneObject = isASceneObject;
        }
    }
}