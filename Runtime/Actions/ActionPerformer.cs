using System.Collections.Generic;

namespace BB.BardicFramework.Actions
{
    public abstract class ActionPerformer : ActorModule { }

    public abstract class ActionPerformer<TAction, TPerformer, TRuntime> : ActionPerformer 
        where TAction : Action<TAction, TPerformer, TRuntime>
        where TPerformer : ActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : ActionRuntime<TAction, TPerformer, TRuntime>
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
            for(int i =0; i < activeRuntimes.Count; i++)
            {
                if (!activeRuntimes[i].IsInProgress)
                {
                    //Debug.Log("Cleaning Up " + activeRuntimes[i].Action.name + " runtime. It is no longer running.");
                    activeRuntimes[i].Stop();
                    activeRuntimes.RemoveAt(i--);
                    continue;
                }
                activeRuntimes[i].Update();
            }
        }

        public virtual void Perform(TAction action)
        {
            activeRuntimes.Add(action.GetRuntime((TPerformer)this));            
        }
    }

}