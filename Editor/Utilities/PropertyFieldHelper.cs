//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BardicBytes.BardicFrameworkEditor.Utilities
{
    public class PropertyFieldHelper
    {
        private Dictionary<string, SerializedProperty> targetProps;
        private SerializedObject serializedObject;
        private bool foldoutOpen;

        public PropertyFieldHelper(SerializedObject serializedObject)
        {
            this.serializedObject = serializedObject;
            targetProps = new Dictionary<string, SerializedProperty>();
            foldoutOpen = false;
        }

        public void DrawPropFields(params string[] pPath) => DrawPropFields((pPath != null) && (pPath.Length > 0), pPath);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializedObject">The object to be drawn. </param>
        /// <param name="targetProps"></param>
        /// <param name="drawUnspecified">Draws all unspecified properties inside a foldout.</param>
        /// <param name="foldoutOpen">irrelevent if drawUnspecified is false</param>
        /// <param name="pPath"></param>
        public void DrawPropFields(bool drawUnspecifiedparams, params string[] pPath)
        {
            serializedObject.Update();
            bool changed = false;
            for (int i = 0; i < pPath.Length; i++)
            {
                changed |= DrawProp(pPath[i]);
            }
            foldoutOpen = DrawFoldout(drawUnspecifiedparams, pPath.Length); ;
            serializedObject.ApplyModifiedProperties();

            bool DrawProp(string propPath)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(GetProp(serializedObject, propPath), true);
                return EditorGUI.EndChangeCheck();
            }

            SerializedProperty GetProp(SerializedObject serializedObject, string propPath)
            {
                var sp = serializedObject.FindProperty(propPath);
                if (sp != null && !targetProps.ContainsKey(propPath)) targetProps.Add(propPath, sp);
                if (targetProps.ContainsKey(propPath)) return targetProps[propPath];
                throw new System.Exception(propPath + " not found in "+serializedObject.ToString());
            }

            bool DrawFoldout(bool drawUnspecified, int specifiedCount)
            {
                GUILayout.FlexibleSpace();
                if (specifiedCount > 0 && drawUnspecified)
                {
                    foldoutOpen = EditorGUILayout.Foldout(drawUnspecified && foldoutOpen, "Everything Else", true);
                }
                else if (specifiedCount == 0)
                {
                    foldoutOpen = true;
                }
                if (!foldoutOpen) return false;
                EditorGUI.indentLevel++;
                GUILayout.BeginVertical("box");
                var nextProp = serializedObject.GetIterator();
                bool goDeeper = true;
                while (nextProp.NextVisible(goDeeper))
                {
                    goDeeper = false;
                    if (!targetProps.ContainsKey(nextProp.name) && nextProp.name != "m_Script")
                    {
                        EditorGUILayout.PropertyField(nextProp);
                    }
                }
                GUILayout.EndVertical();
                EditorGUI.indentLevel--;
                return foldoutOpen;
            }
        }
    }
}
