//alex@bardicbytes.com
using BardicBytes.BardicFramework;
using BardicBytes.BardicFramework.EventVars;
using UnityEditor;
using UnityEngine;

namespace BardicBytes.BardicFrameworkEditor
{
    [CustomPropertyDrawer(typeof(EventVarField), true)]
    public class EventVarFieldDrawer : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty EVFieldProperty, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, EVFieldProperty);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            //EditorGUI.indentLevel = 0;

            var r = new Rect(position.x, position.y, 200, position.height);
            SerializedProperty evProp = EVFieldProperty.FindPropertyRelative("srcEV");
            
            
            if (evProp.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(r, EVFieldProperty.FindPropertyRelative("fallbackValue"), GUIContent.none);
                r = new Rect(position.x + r.width + 5, position.y, position.width - r.width - 5, position.height);
            }
            else
            {
                r = position;
            }
            EditorGUI.PropertyField(r, evProp, GUIContent.none);
            EditorGUI.indentLevel = indent;

            SerializedProperty moduleProp = EVFieldProperty.FindPropertyRelative("module");

            if (EVFieldProperty.serializedObject.targetObject != moduleProp.objectReferenceValue)
            {
                var module = EVFieldProperty.serializedObject.targetObject as ActorModule;
                moduleProp.objectReferenceValue = module;
                EditorUtility.SetDirty(module);
            }
            EditorGUI.EndProperty();
        }
    }
}