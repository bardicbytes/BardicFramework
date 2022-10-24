//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using System;
using UnityEngine;
using UnityEngine.Events;

namespace BB.BardicFramework.EventVars
{
    public interface IMinMax<T>
    {
        T MinValue { get; set; }
        T MaxValue { get; set; }

        T MinMaxClamp(T value);
    }

    [CreateAssetMenu(menuName = Prefixes.EV+ "EvenVar without Data")]
    public class EventVar : ScriptableObject
    {
        [SerializeField]
        protected UnityEvent untypedEvent;

        protected float lastRaiseTime;
        protected bool isInitialized = false;

        public EventVar CloneSource { get; protected set; } = null;
        public EventVarInstancer InstanceOwner { get; protected set; } = null;
        public bool IsActorInstance { get; protected set; } = false;

        public virtual bool HasValue { get => false; }
        public virtual Type StoredValueType => default;
        public virtual Type OutputValueType => default;

        public virtual object UntypedStoredValue { get; protected set; }

        public virtual object UntypedOutputValue => UntypedStoredValue;


        [field: HideInInspector]
        [field: SerializeField]
        public string GUID { get; private set; } = null;

        int lc = 0;

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(GUID) || GUID == name)
            {
                var p = UnityEditor.AssetDatabase.GetAssetPath(this);
                GUID = UnityEditor.AssetDatabase.AssetPathToGUID(p);
            }
#endif
        }

        protected virtual void OnValidate()
        {
            lastRaiseTime = 0;
            lc = 0;
        }

        protected virtual void OnEnable()
        {
            if (untypedEvent != null)
                untypedEvent.RemoveAllListeners();
            lc = 0;

            isInitialized = false;
            Initialize();
        }

        public virtual T Clone<T>(EventVarInstancer owner) where T : EventVar
        {
            var c = Instantiate(this) as T;
            c.CloneSource = this;
            c.InstanceOwner = owner;
            c.IsActorInstance = true;
            return c;
        }

        protected virtual void Initialize()
        {
            if (isInitialized || !Application.isPlaying) return;
            lastRaiseTime = 0;
            isInitialized = true;
        }

        public virtual void SetInitialValue(EventVarInstanceField bc)
        {
            throw new NotImplementedException("There's no reason to instance an event var without without data.");
            
        }

        public virtual EventVarInstanceField CreateInstanceConfig()
        {
            throw new NotImplementedException("There's no reason to instance an event var without without data.");
        }

        public virtual OutT Eval<InT, OutT>()
        {
            return Eval<InT, OutT>((InT)UntypedStoredValue);
        }
        public virtual OutT Eval<InT, OutT>(InT inValue)
        {
            return ((BaseGenericEventVar<InT, OutT, EvaluatingEventVar<InT, OutT>>)this).Eval(inValue);
        }

        public override string ToString()
        {
            return name + "? " + UntypedStoredValue;
        }

        public virtual void Raise()
        {
            Initialize();
            lastRaiseTime = Time.realtimeSinceStartup;

            try
            {
                untypedEvent.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(name + "");
                throw ex;
            }
        }

        public virtual void AddListener(UnityAction action)
        {
            Initialize();

            Debug.Assert(action != null);
            untypedEvent.AddListener(action);
            lc++;
        }

        public virtual void RemoveListener(UnityAction action)
        {
            Initialize();
            untypedEvent.RemoveListener(action);
            lc--;
            if (lc <= 0) Debug.LogWarning("removing no listener");
        }



    }
}