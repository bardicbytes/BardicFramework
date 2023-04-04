//alex@bardicbytes.com
using UnityEditor;
using UnityEngine;

namespace Bardic.BardicFrameworkEditor.Utilities
{
    public class GUIDLookupWindow : EditorWindow
    {
        [MenuItem("Bardic/Guid Lookup...")]
        private static void InitWindow()
        {
            var window = GetWindow<GUIDLookupWindow>(true, "Guid Lookup", true);
            window.minSize = new Vector2(450, 150);
            window.maxSize = new Vector2(700, 150);
            window.Show();
        }
        private string inputText, pathDisp;
        private Object selectedObject;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Paste GUID for asset ref");
            GUILayout.BeginHorizontal();
            inputText = EditorGUILayout.TextField("", inputText);
            pathDisp = AssetDatabase.GUIDToAssetPath(inputText);
            GUILayout.EndHorizontal();
            if (pathDisp != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10f);
                GUILayout.Label(pathDisp);
                UnityEngine.Object o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathDisp);
                GUILayout.FlexibleSpace();
                if (o != null && GUILayout.Button(o.name)) Selection.activeObject = o;
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("");
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("and/or");
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            selectedObject = EditorGUILayout.ObjectField("Select Asset for GUID", selectedObject, typeof(Object), false);
            GUILayout.EndHorizontal();
            if (selectedObject != null)
            {
                var p = AssetDatabase.GetAssetPath(selectedObject);
                GUILayout.TextField(AssetDatabase.AssetPathToGUID(p));
                GUILayout.Label("");
            }
            else
            {
                GUILayout.TextField(AssetDatabase.AssetPathToGUID(""));
                GUILayout.Label("");
            }
            GUILayout.EndVertical();
        }
    }
}