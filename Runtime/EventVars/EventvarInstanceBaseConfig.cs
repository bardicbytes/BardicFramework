//alex@bardicbytes.com
using BB.BardicFramework.EventVars;
using UnityEditor;
using UnityEngine;

namespace BB.BardicFramework.EventVars
{
    [System.Serializable]
    public class EventVarInstanceField
    {

        const string backingFieldpost = "k__BackingField";

        [HideInInspector]
        public string editorName;
        [SerializeField]
        protected EventVar src;
        [SerializeField]
        protected EventVar baseRuntimeInstance;
        
        [field: SerializeField] public string StringValue { get; protected set; }
        [field: SerializeField] public int IntValue { get; protected set; }
        [field: SerializeField] public float FloatValue { get; protected set; }
        [field: SerializeField] public bool BoolValue { get; protected set; }
        [field: SerializeField] public Vector3 Vector3Value { get; protected set; }
        [field: SerializeField] public Vector2Int Vector2IntValue { get; protected set; }
        [field: SerializeField] public UnityEngine.Object UnityObjectValue { get; protected set; }
        [field:SerializeField] public System.Object SystemObjectValue { get; protected set; }
        public virtual EventVar BaseRuntimeInstance => baseRuntimeInstance;
        
        private string selector = null;
        [SerializeField]
        private string DEBUG_selstring = "?";

#if UNITY_EDITOR
        public virtual void PropField(Rect position, UnityEditor.SerializedProperty evifProp)
        {
            selector = src.StoredValueType.FullName;
            DEBUG_selstring = selector+"";
            bool didDraw = false;

            if (selector == typeof(string).FullName) DrawPF("StringValue");
            else if (selector == typeof(int).FullName) DrawPF("IntValue");
            else if (selector == typeof(float).FullName) DrawPF("FloatValue");
            else if (selector == typeof(bool).FullName) DrawPF("BoolValue");
            else if (selector == typeof(Vector3).FullName) DrawPF("Vector3Value");
            else if (selector == typeof(Vector2Int).FullName) DrawPF("Vector2IntValue");
            if (didDraw) return;
            if (selector == typeof(UnityEngine.Object).FullName)
            {
                EditorGUI.PropertyField(position, FindPropRel("UnityObjectValue"), true);
                return;
            }

            if (selector == typeof(System.Object).FullName)
            {
                EditorGUI.PropertyField(position, FindPropRel("SystemObjectvalue"), true);
                return;
            }

            EditorGUI.LabelField(position, "No Instancing Available for "+ src.StoredValueType.Name);

            void DrawPF(string propname)
            {
                var bp = FindPropRel(propname);
                if (bp != null) EditorGUI.PropertyField(position, bp);
                else EditorGUI.LabelField(position, propname+" null. "+evifProp.propertyPath);

                didDraw = true;
            }

            SerializedProperty FindPropRel(string propName)
            {
                var s = string.Format("<{1}>{0}", backingFieldpost,propName);
                SerializedProperty sp = evifProp.Copy();
                if (sp.name == s) return sp;
                while (sp.Next(true))
                {
                    if(sp.name == s) return sp;
                    //Debug.Log(sp.name+"");
                }
                return null;
            }
        }
#endif

        public EventVar SRC => src;
    }

    [System.Serializable]
    public class EventvarInstanceField<InT, OutT, EvT, BCT> : EventVarInstanceField where EvT : BaseGenericEventVar<InT, OutT, EvT> where BCT : EventvarInstanceField<InT, OutT, EvT, BCT>
    {
        public static implicit operator EvT(EventvarInstanceField<InT, OutT, EvT, BCT> c) => c == null ? null : c.RuntimeInstance;
        public EvT RuntimeInstance
        {
            get
            {
                return this.baseRuntimeInstance as EvT;
            }
            set
            {
                this.baseRuntimeInstance = value;
            }
        }

        public InT InitialValue 
        { 
            get
            {
                return ((EvT)src).InitialValue;
            }
            set
            {

            }
        }
        public EventvarInstanceField() => RuntimeInstance = null;
        public EventvarInstanceField(EvT ev) => RuntimeInstance = ev;
        public void SetSrc(EvT evSrc)
        {
            base.src = evSrc;
            var selector = evSrc.StoredValueType.FullName;

            if (selector == typeof(string).FullName)
            {
                StringValue = evSrc.StoredValue as string;
                return;
            }

            if (selector == typeof(int).FullName)
            {
                IntValue = (int)src.UntypedStoredValue;
                return;

            }
            if (selector == typeof(float).FullName)
            {
                FloatValue = (float)src.UntypedStoredValue;
                return;
            }
            if (selector == typeof(bool).FullName)
            {
                BoolValue = (bool)src.UntypedStoredValue;
                return;
            }
            if (selector == typeof(Vector3).FullName)
            {
                Vector3Value = (Vector3)src.UntypedStoredValue;
                return;
            }
            if (selector == typeof(Vector2Int).FullName)
            {
                Vector2IntValue = (Vector2Int)src.UntypedStoredValue;
                return;
            }
            if (selector == typeof(UnityEngine.Object).FullName)
            {
                UnityObjectValue = src.UntypedStoredValue as UnityEngine.Object;
                return;
            }
            if (selector == typeof(System.Object).FullName)
            {
                SystemObjectValue = src.UntypedStoredValue as System.Object;
                return;
            }
        }

#if UNITY_EDITOR
        public override void PropField(Rect position, SerializedProperty evifProp)
        {
            GenericPropField(position, evifProp);
        }
        public virtual void GenericPropField(Rect position, UnityEditor.SerializedProperty evifProp)
        {
            UnityEditor.EditorGUI.LabelField(position, "GenericPropField ");
        }
#endif
    }
}