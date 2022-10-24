//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace BardicBytes.BardicFramework.Core.EventVars
{
    public abstract class GenericEventVar<T> : AutoEvaluatingEventVar<T,T> { }

    public abstract class AutoEvaluatingEventVar<InT,OutT> : EvaluatingEventVar<InT, OutT> where InT : OutT
    {
        public override OutT Eval(InT val) => val; //a lil shortcut
    }

    public abstract class EvaluatingEventVar<InT, OutT> : BaseGenericEventVar<InT, OutT, EvaluatingEventVar<InT,OutT>>{}

    public abstract class BaseGenericEventVar<InT, OutT, EvT> : EventVar where EvT : BaseGenericEventVar<InT, OutT, EvT>
    {
        [Serializable]
        public class EventVarInstanceConfig : EventvarInstanceField<InT, OutT, EvT, EventVarInstanceConfig>
        {
#if UNITY_EDITOR
            public override void GenericPropField(Rect position, SerializedProperty evifProp)
            {
                base.GenericPropField(position, evifProp);
            }
#endif
        }

        /// <summary>
        /// The Actor's instance Value, the source asset's Value, or this field's fallback value; in that order.
        /// </summary>
        [Serializable]
        public class Field : GenericEventVarField<InT, OutT, EvT> { }

        [Serializable]
        public class UnityEvent : UnityEvent<OutT> { }

        [SerializeField]
        protected UnityEvent typedEvent = default;
        [SerializeField]
        protected InT initialValue = default;

        [Space]
        [SerializeField]
        protected bool logValueChange = false;
        [field: SerializeField]
        public bool RequireInstancing { get; protected set; }

        [Space]
        [SerializeField]
        protected bool resetValueOnDatalessRaise = true;
        [SerializeField]
        protected bool abortRaiseForIdenticalData = false;
        [SerializeField]
        [Tooltip("Also prevents reset/dataless raises if current value matches initial value")]
        protected bool requireData = false;
        [SerializeField]
        protected bool autoLock = false;
        [SerializeField]
        protected bool invokeNewListeners = false;

        [SerializeField]
        [HideInInspector]
        protected bool raiseOnInit = false;

        protected List<Actor> activeInstancers;
        protected bool isLocked = false;
        public override object UntypedOutputValue => Value;

        public InT InitialValue => initialValue;
        public bool IsLocked => isLocked;
        public InT StoredValue
        {
            get
            {
                var b = base.UntypedStoredValue;
                InT v = default;
                if (b != null) v = (InT)b;
                return v;
            }
        }
        public OutT Value
        {
            get
            {
                Debug.Assert(!RequireInstancing || IsActorInstance, name+" "+(IsActorInstance ? "Is ActorInst": "Not ActorInst"));
                return Eval(StoredValue);
            }
        }
        public override bool HasValue { get => true; }
        public override Type StoredValueType => typeof(InT);
        public override Type OutputValueType => typeof(OutT);

        protected override void OnValidate()
        {
            if (StoredValue == null
                || (StoredValue != null && !StoredValue.Equals(initialValue))) UntypedStoredValue = initialValue;
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            UntypedStoredValue = initialValue;
            isLocked = false;
            isInitialized = false;

            if (typedEvent != null) typedEvent.RemoveAllListeners();

            base.OnEnable();
        }
        public virtual OutT Eval(InT val) => base.Eval<InT, OutT>(val);
        /// <summary>
        /// Just use the Value property.
        /// </summary>
        /// <returns>this.Value</returns>
        public OutT Eval() => Value;


        protected override void Initialize()
        {
            if (isInitialized || !Application.isPlaying) return;
            base.Initialize();

            if (false && raiseOnInit) Raise(initialValue);
            else SetInitialValue(initialValue);
        }

        public abstract InT To(EventVarInstanceField bc);

        public override void SetInitialValue(EventVarInstanceField bc)
        {
            InT v = default;
            v = To(bc);
            SetInitialValue(v);
        }

        public void SetInitialValue(InT initialValue)
        {
            this.initialValue = initialValue;
            if (isInitialized) ChangeCurrentValue(initialValue);
            else UntypedStoredValue = initialValue;
        }


        public override string ToString() => GetValueString();

        public virtual string GetValueString()
        {
            return StoredValue == null ? "null value" : StoredValue.ToString();
        }

        public void Lock() => isLocked = true;

        /// <summary>
        /// Clones the source EventVar asset, and puts it in a config
        /// </summary>
        /// <returns>A new EventvarInstanceConfig upcast as a BaseConfig</returns>
        public override EventVarInstanceField CreateInstanceConfig()
        {
            var v = new EventVarInstanceConfig();

            //if this isn't a clone, we're validating assets
            if (this.IsActorInstance)
            {
                v.RuntimeInstance = (EvT)this;
                v.SetSrc((EvT)this.CloneSource);
            }
            else
            {
                v.SetSrc((EvT)this);
            }
            return v;
        }

        public override void Raise()
        {
            Initialize();
            Debug.Assert(!RequireInstancing || IsActorInstance);
            if (requireData
                && resetValueOnDatalessRaise
                && StoredValue.Equals(initialValue)) return;
            if (resetValueOnDatalessRaise) ChangeCurrentValue(initialValue);
            if (requireData) return;
            this.Raise(InitialValue);
            base.Raise();
        }

        public virtual void RaisePassthrough(GenericEventVar<InT> typedEventVar)
        {
            if (typedEventVar == this) throw new System.Exception("No Recursive EventVar Raising!");
            Raise(typedEventVar.Value);
        }

        public virtual void Raise(InT data)
        {
            Initialize();
            Debug.Assert(!RequireInstancing || IsActorInstance);
            if (IsLocked && abortRaiseForIdenticalData && StoredValue.Equals(data)) Debug.LogWarning("This locked event var can't be raised with identical data. " + name);
            if (IsLocked && !StoredValue.Equals(data)) Debug.LogError("Locked event vars are read only and cannot be raised new data" + name);
            if (IsLocked || abortRaiseForIdenticalData && StoredValue.Equals(data)) return;
            if (autoLock) Lock();
            ChangeCurrentValue(data);
            typedEvent.Invoke(Value);
            base.Raise();
        }

        private void ChangeCurrentValue(InT data)
        {
            if (Debug.isDebugBuild && logValueChange && !data.Equals(StoredValue))
            {
                Debug.Log(name + " value changed. " + StoredValue + "->" + data.ToString());
            }

            UntypedStoredValue = data;
        }

        public virtual void AddListener(UnityAction<OutT> action)
        {
            Debug.Assert(!RequireInstancing || IsActorInstance);

            AddInitialListener(action);

            if (invokeNewListeners)
            {
                action.Invoke(Value);
            }
        }

        public void AddInitialListener(UnityAction<OutT> action)
        {
            Debug.Assert(!RequireInstancing || IsActorInstance);

            Initialize();
            typedEvent.AddListener(action);
        }

        public virtual void RemoveListener(UnityAction<OutT> action)
        {
            Initialize();
            typedEvent.RemoveListener(action);
        }

#if UNITY_EDITOR
        public virtual InT PropField(Rect position, UnityEditor.SerializedProperty rawProp)
        {
            EditorGUI.LabelField(position, initialValue.ToString());
            return default;
        }
#endif
    }
}