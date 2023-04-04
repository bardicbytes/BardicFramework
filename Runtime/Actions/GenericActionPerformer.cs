using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public abstract class GenericActionPerformer<TAction, TPerformer, TRuntime> : ActionPerformer
        where TAction : GenericAction<TAction, TPerformer, TRuntime>
        where TPerformer : GenericActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : GenericActionRuntime<TAction, TPerformer, TRuntime>
    {

        [SerializeField]
        protected MonoBehaviour serializedInputSource;

        public IProvideActionInput InputSource => serializedInputSource == null ? null : serializedInputSource as IProvideActionInput;


        protected List<TAction> actionHistory;
        protected List<TRuntime> activeRuntimes;

        protected override void OnValidate()
        {
            base.OnValidate();

            if (serializedInputSource == null)
            {
                serializedInputSource = GetComponent<IProvideActionInput>() as MonoBehaviour;
            }
            else if (!(serializedInputSource is IProvideActionInput))
            {
                serializedInputSource = serializedInputSource.GetComponent<IProvideActionInput>() as MonoBehaviour;
            }

            if (!(serializedInputSource is IProvideActionInput))
            {
                Debug.LogWarning("serializedInputSource must implement IProvideActionInput");
                serializedInputSource = null;
            }
        }

        protected void Awake()
        {
            actionHistory = new List<TAction>();
            activeRuntimes = new List<TRuntime>();
        }

        protected override void ActorUpdate()
        {

            for (int i = 0; i < activeRuntimes.Count; i++)
            {
                if (!activeRuntimes[i].IsInProgress)
                {
                    activeRuntimes[i].StopAction();
                    activeRuntimes.RemoveAt(i--);
                    continue;
                }
                activeRuntimes[i].Update();
            }
        }

        public virtual void Perform(TAction action)
        {
            Debug.Assert(action != null);
            TRuntime rt = action.CreateRuntime((TPerformer)this);
            activeRuntimes.Add(rt);
        }
    }

}