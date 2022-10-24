using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BardicBytes.BardicFramework.Core.Editor
{
    public static class EditorHelper
    {
        public static object GetValue(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
#pragma warning disable 168
                try
                {
                    var type = obj.GetType();
                    field = type.GetField(path);
                    obj = field.GetValue(obj);
                }
                catch(System.NullReferenceException nre)
                {
                    //Debug.LogWarning(nre.Message);
                    return null;
                }
#pragma warning restore 168

            }
            return obj;
        }

        // Sets value from SerializedProperty - even if value is nested
        public static void SetValue(this SerializedProperty property, object val)
        {
            object obj = property.serializedObject.targetObject;

            List<KeyValuePair<FieldInfo, object>> list = new List<KeyValuePair<FieldInfo, object>>();

            FieldInfo field = null;
            foreach (var path in property.propertyPath.Split('.'))
            {
                var type = obj.GetType();
                field = type.GetField(path);
                list.Add(new KeyValuePair<FieldInfo, object>(field, obj));
                obj = field.GetValue(obj);
            }

            // Now set values of all objects, from child to parent
            for (int i = list.Count - 1; i >= 0; --i)
            {
                list[i].Key.SetValue(list[i].Value, val);
                // New 'val' object will be parent of current 'val' object
                val = list[i].Value;
            }
        }
    }

}