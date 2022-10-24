//alex@bardicbytes.com
using BardicBytes.BardicFramework.Core;
using BardicBytes.BardicFramework.Core.Editor;
using BardicBytes.BardicFramework.Core.EventVars;
using UnityEditor;
using UnityEngine;
using static BardicBytes.BardicFramework.Core.EventVarInstancer;

namespace BardicBytes.BardicFramework.EventVars.Editor
{
    [CustomPropertyDrawer(typeof(EventVarInstanceField), true)]
    public class EventVarInstanceFieldDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var evProp = property.FindPropertyRelative("baseRuntimeInstance");
            var srcProp = property.FindPropertyRelative("src");

            //Debug.Assert(evProp != null, "couldn't find prop runtimeInstance of EventVarInstanceConfig");
            EventVar ev = evProp == null || evProp.objectReferenceValue == null ? null : evProp.objectReferenceValue as EventVar;
            var r = new Rect(position.x, position.y, 75, position.height);
            var r2 = new Rect(position.x + r.width+5, position.y, position.width - r.width - 5, position.height);
            if (ev != null)
            {
                var evSO = new SerializedObject(ev);
                var valProp = evSO.FindProperty("initialValue");
                if (valProp != null) EditorGUI.PropertyField(r, valProp, GUIContent.none);
                evSO.ApplyModifiedProperties();
            }
            else if(srcProp != null)
            {
                //the src has been set to the instancer.
                //var srcSO = new SerializedObject(srcProp.objectReferenceValue);
                //var valProp = srcSO.FindProperty("initialValue");
                EventVarInstanceField bc = (EventVarInstanceField)property.boxedValue;
                if (bc != null)
                {
                    bc.PropField(position, property);
                }
                else
                {
                    EditorGUI.LabelField(position, "bc is null");
                }
            }
            else
            {
                EditorGUI.LabelField(position, "ev+srcProp=null. of "+property.displayName);
            }
            if (evProp != null && ev != null) EditorGUI.PropertyField(r2, evProp, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

    }
}