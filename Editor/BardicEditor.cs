//alex@bardicbytes.com
using BardicBytes.BardicFramework;
using BardicBytes.BardicFrameworkEditor.Utilities;
using UnityEditor;

namespace BardicBytes.BardicFrameworkEditor
{
    public abstract class BardicEditor<T> : Editor where T : UnityEngine.Object, IBardicEditorable
    {
        private PropertyFieldHelper propHelper;
        protected T Target => target as T;
        protected virtual void OnEnable()
        {
            propHelper = new PropertyFieldHelper(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            UnityEngine.Debug.Assert(Target.EditorFieldNames != null);
            UnityEngine.Debug.Assert(propHelper != null);
            propHelper.DrawPropFields(Target.DrawOtherFields, Target.EditorFieldNames);
        }
    }
}
