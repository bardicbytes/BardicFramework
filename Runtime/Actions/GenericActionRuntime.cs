//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;
using static BardicBytes.BardicFramework.SimpleStateMachine;

namespace BardicBytes.BardicFramework.Actions
{
    [System.Serializable]
    public abstract class GenericActionRuntime<TAction, TPerformer, TRuntime> : ActionRuntime
        where TAction : GenericAction<TAction, TPerformer, TRuntime>
        where TPerformer : GenericActionPerformer<TAction, TPerformer, TRuntime>
        where TRuntime : GenericActionRuntime<TAction, TPerformer, TRuntime>
    {
        protected TPerformer actionPerformer;
        protected TAction action;

        public Actor Actor { get { return actionPerformer.Actor; } }
        public TPerformer ActionPerformer { get { return actionPerformer; } }
        public TAction Action { get { return action; } }

        protected float startTime;
        protected int currentPhaseIndex;
        public GenericAction<TAction, TPerformer, TRuntime>.PhaseData CurrentPhaseData { get { return action.GetPhaseData(currentPhaseIndex); } }

        public string WhatAndWhy { get { return Action.FullName+" is in the state "+stateMachine.CurrentState.name+" because... "+stateMachine.CurrentState.Why; } }

        public GenericActionRuntime() {} 

        public GenericActionRuntime(TAction action, TPerformer actionPerformer)
        {
            Debug.Assert(action != null, string.Format("action: {0}, prformer:{1}", action, actionPerformer));
            this.actionPerformer = actionPerformer;
            this.action = action;
            //if(Debug.isDebugBuild) Debug.Log("Action Started: "+this.action);
            SetNewStateMachine();
            StartAction();
        }
        
        public override void Update()
        {
            base.Update();
            if (IsInProgress)
                CurrentPhaseData.HandleOnUpdate((TRuntime)this);
        }

        protected override SimpleStateMachine BuildStateMachine()
        {
            if (action == null || actionPerformer == null) return null;
            List<SimpleState> phases = new List<SimpleState>();
            //Debug.Assert(action != null);
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

        public override void StopAction()
        {
            if(IsInProgress) Debug.Log(Action.FullName + " Stopping while in progress. DurationAlive: " + (Time.time - startTime), Action);
            base.StopAction();
        }

        #region state event handles
        protected void HandleOnEnterState(SimpleState state)
        {
            CurrentPhaseData.HandleOnEnter((TRuntime)this);
            ProcessAction();
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