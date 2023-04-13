using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using KorYmeLibrary.Attributes;

namespace KorYmeLibrary.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class KorYmeInspector : UnityEditor.Editor
    {
        private IEnumerable<MethodInfo> _allMethods;

        private void OnEnable()
        {
            _allMethods = ReflectionUtility.GetAllMethods(target, x => x.GetCustomAttributes(typeof(MethodAttribute),true).Length > 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawButtons();
        }
        private void DrawButtons()
        {

        }
    }
}
