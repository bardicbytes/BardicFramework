//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;

namespace BB.BardicFramework
{
    /// <summary>
    /// This is a good state machine to use if everything can/should be defined procedurally.
    /// </summary>
    public class SimpleStateMachine
    {
        private List<SimpleState> states;
        private SimpleState currentState, next;

        public SimpleState CurrentState { get; protected set; }
        public bool HasState => CurrentState != null;

        public SimpleStateMachine(params SimpleState[] states)
        {
            if (states.Length == 0) return;
            this.states = new List<SimpleState>(states);            
            next = this.states[0];
            currentState = null;
        }

        public void Start()
        {
            if(currentState == null) GoToNext();
        }

        public void Tick()
        {
            if (HasState) GoToNext();
        }

        public void GoToNext(bool forceNext = false)
        {
            if(forceNext || currentState == null || (currentState.IsInternallyComplete)) GoTo(next);
        }

        public void GoTo(SimpleState nextState)
        {
            if (currentState != null) currentState.Exit();
            currentState = nextState;
            if (currentState != null)
            {
                SetNext(currentState.next);
                currentState.Enter();
            }
        }
        
        public void SetNext(SimpleState nextState)
        {
            this.next = nextState;
        }

        public override string ToString()
        {
            string text = "";
            for(int i = 0; i < states.Count; i++)
            {
                text += states[i].name;
                if (i != states.Count - 1) text += ", ";
            }
            return "<B>StateMachine!</B> Current: "+CurrentState+"\nstates: " + text;
        }

        public class SimpleState
        {
            public delegate void StateChange(SimpleState state);
            public string name;
            public StateChange onEnter, onExit;
            [System.NonSerialized]
            public SimpleState next;

            public virtual bool IsInternallyComplete { get { return true; } }
            public virtual string Why { get { return "Always Is!"; } }

            public override string ToString() => string.Format("{0} ({1}) Why? {2}", name, GetType().Name,Why);

            public virtual void Enter() => onEnter.Invoke(this);
            public virtual void Exit() => onExit.Invoke(this);
        }

        public abstract class ConditionalState : SimpleState
        {
            public delegate bool CompletionCheck(ConditionalState state);
            public CompletionCheck onCompletionCheck;

            public override bool IsInternallyComplete => base.IsInternallyComplete && (onCompletionCheck == null || onCompletionCheck.Invoke(this));
            public override string Why => "State has not passed completion check. delegate count:" + (onCompletionCheck?.GetInvocationList().Length);
        }
        public class TimedState : SimpleState
        {
            public float duration;
            protected float startTime, endTime;

            public override bool IsInternallyComplete => base.IsInternallyComplete && Time.time >= endTime;

            public override string Why => string.Format("Time.time({0}) < endTime ({1})", Time.time, endTime);

            public float NormalizedTime => duration == 0 ? 0 : 1 - (endTime - Time.time) / duration;

            public override void Enter()
            {
                startTime = Time.time;
                endTime = startTime + duration;
                base.Enter();
            }

            public override void Exit()
            {
                base.Exit();
            }
        }
    }
}