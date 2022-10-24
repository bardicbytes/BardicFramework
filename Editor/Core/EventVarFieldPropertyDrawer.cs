using BardicBytes.BardicFramework.Core.EventVars;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BardicBytes.BardicFramework.EventVars.Editor
{
    [CustomPropertyDrawer(typeof(BaseEventVarField), true)]
    public class EventVarFieldDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            var r = new Rect(position.x, position.y, 200, position.height);
            SerializedProperty evProp = property.FindPropertyRelative("srcEV");
            if (evProp.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(r, property.FindPropertyRelative("fallbackValue"), GUIContent.none);
                r = new Rect(position.x + r.width + 5, position.y, position.width - r.width - 5, position.height);
            }
            else
            {
                r = position;
            }
            EditorGUI.PropertyField(r, evProp, GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}