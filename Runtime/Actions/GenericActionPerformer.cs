using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public abstract class GenericActionPerformer<TAction, TPerformer, TRuntime> : ActionPerformer
        where TAction : GenericAction<TAction, TPerformer, TRuntime>
        where TPerformer : GenericActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : GenericActionRuntime<TAction, TPerformer, TRuntime>
    {

        protected List<TAction> actionHistory;
        protected List<TRuntime> activeRuntimes;

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