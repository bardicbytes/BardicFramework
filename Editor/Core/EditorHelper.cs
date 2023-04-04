using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace BardicBytes.BardicFrameworkEditor
{
    public static class EditorHelper
    {
        //https://forum.unity.com/threads/get-a-general-object-value-from-serializedproperty.327098/
        public static object GetValue(this SerializedProperty prop)
        {
            object obj = prop.serializedObject.targetObject;

            var paths = prop.propertyPath.Split('.');
            for (int i = 0; i < paths.Length; i++)
            {
#pragma warning disable 168
                try
                {
                    obj = obj.GetType().GetField(paths[i]).GetValue(obj);
                }
                catch (System.NullReferenceException nre)
                {
                    //Debug.LogWarning(nre.Message);
                    return null;
                }
#pragma warning restore 168

            }
            return obj;
        }

        public static void SetValue(this SerializedProperty prop, object val)
        {
            object obj = prop.serializedObject.targetObject;

            List<KeyValuePair<FieldInfo, object>> propsList = new List<KeyValuePair<FieldInfo, object>>();

            FieldInfo field = null;
            foreach (var path in prop.propertyPath.Split('.'))
            {
                field = obj.GetType().GetField(path);
                propsList.Add(new KeyValuePair<FieldInfo, object>(field, obj));
                obj = field.GetValue(obj);
            }

            var v = val;
            for (int i = 0; i < propsList.Count; i++)
            {
                propsList[i].Key.SetValue(propsList[i].Value, v);
                v = propsList[i].Value;
            }
        }
    }

}