//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;
using static BardicBytes.BardicFramework.SimpleStateMachine;

namespace BardicBytes.BardicFramework.Actions
{
    [System.Serializable]
    public abstract class ActionRuntime
    {
        public class ActionState : TimedState{}

#if UNITY_EDITOR
#pragma warning disable 414
        [SerializeField]
        private string inspectorName;
#pragma warning restore 414
#endif
        [SerializeField]
        protected SimpleStateMachine stateMachine;
        public bool IsInProgress { get { return stateMachine != null && stateMachine.HasState; } }

        public float PhaseNormTime
        {
            get
            {
                return ((ActionState)stateMachine.CurrentState).NormalizedTime;
            }
        }

        public ActionRuntime()
        {
            stateMachine = GetStateMachine();
            stateMachine.Start();
        }

        public virtual void Start()
        {
            stateMachine.Start();
        }

        public virtual void Stop()
        { 
            if (IsInProgress)
                Debug.Log(this + " Stopped Prematurely.");
            stateMachine = null;
        }

        public virtual void Update()
        {
            Process();
        }

        //frame independant
        public virtual void Process()
        {
            stateMachine.Tick();
        }

        protected abstract SimpleStateMachine GetStateMachine();
        protected TimedState[] states;
    }

    [System.Serializable]
    public abstract class ActionRuntime<TAction, TPerformer, TRuntime> : ActionRuntime
        where TAction : Action<TAction, TPerformer, TRuntime>
        where TPerformer : ActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : ActionRuntime<TAction, TPerformer, TRuntime>
    {
        protected TPerformer actionPerformer;
        protected TAction action;

        public Actor Actor { get { return actionPerformer.Actor; } }
        public TPerformer ActionPerformer { get { return actionPerformer; } }
        public TAction Action { get { return action; } }

        protected float startTime;
        protected int currentPhaseIndex;
        public Action<TAction, TPerformer, TRuntime>.PhaseData CurrentPhaseData { get { return action.GetPhaseData(currentPhaseIndex); } }

        public string WhatAndWhy { get { return Action.FullName+" is in the state "+stateMachine.CurrentState.name+" because... "+stateMachine.CurrentState.Why; } }


        public ActionRuntime(TAction action, TPerformer actionPerformer) : base()
        {
            Debug.Log("Action Started: "+action);
            this.actionPerformer = actionPerformer;
            this.action = action;
        }

        public override void Update()
        {
            base.Update();
            if (IsInProgress)
                CurrentPhaseData.HandleOnUpdate((TRuntime)this);
        }

        protected override SimpleStateMachine GetStateMachine()
        {
            List<SimpleState> phases = new List<SimpleState>();
            for(int i = action.PhaseDataCount -1; i >= 0; i--)
            {
                SimpleState next = null;
                if (i + 1 < action.PhaseDataCount) next = phases[i + 1];
                phases.Add(new TimedState
                {
                    name = action.GetPhaseData(i).name,
                    onEnter = HandleOnEnterState,
                    onExit = HandleOnExitState,
                    next = next,
                    duration = action.GetPhaseData(i).duration
                });
            }
            
            return new SimpleStateMachine(phases.ToArray());
        }

        public override void Stop()
        {
            if(IsInProgress) Debug.Log(Action.FullName + " Stopping while in progress. DurationAlive: " + (Time.time - startTime), Action);
            base.Stop();
        }

        #region state event handles
        protected void HandleOnEnterState(SimpleState state)
        {
            CurrentPhaseData.HandleOnEnter((TRuntime)this);
            Process();
        }

        protected void HandleOnExitState(SimpleState state)
        {
            CurrentPhaseData.HandleOnExit((TRuntime)this);
        }
        #endregion

        public override string ToString()
        {
            return action.name+"\n"+stateMachine.ToString();
        }
    }
}