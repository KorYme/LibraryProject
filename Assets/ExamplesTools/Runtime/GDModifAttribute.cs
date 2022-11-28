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
    
        public enum TYPEOBJECTUNITY
        {
            INSTANCE,
            PREFAB,
        }

        public TYPEOBJECTUNITY _type;

        public GDModifAttribute() 
        {
            _type = TYPEOBJECTUNITY.INSTANCE;
        }

        public GDModifAttribute(TYPEOBJECTUNITY type)
        {
            _type = type;
        }
    }
}