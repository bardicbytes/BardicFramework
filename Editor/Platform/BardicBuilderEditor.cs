//alex@bardicbytes.com
using UnityEditor;
using UnityEngine;

namespace BardicBytes.BardicFrameworkEditor.Platform
{
    [CustomEditor(typeof(BardicBuilder))]
    public class BardicBuilderEditor : BardicEditor<BardicBuilder>
    {
        protected override void OnInspectorGUIBeforeOtherFields()
        {
            if(GUILayout.Button("Build Now"))
            {
                Target.BuildGame();
            }
        }
        
    }
}