//alex@bardicbytes.com
using UnityEngine;
using static BardicBytes.BardicFramework.SimpleStateMachine;

namespace BardicBytes.BardicFramework.Actions
{
    [System.Serializable]
    public abstract class ActionRuntime
    {
        public class ActionState : TimedState { }

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

        protected void SetNewStateMachine() => stateMachine = BuildStateMachine();

        public virtual void StartAction()
        {
            stateMachine.Start();
        }

        public virtual void StopAction()
        {
            if (IsInProgress)
            {
                Debug.Log(this + " Stopped Prematurely.");
            }

            stateMachine = null;
        }

        public virtual void Update()
        {
            ProcessAction();
        }

        //frame independant
        public virtual void ProcessAction()
        {
            stateMachine.Tick();
        }

        protected abstract SimpleStateMachine BuildStateMachine();
        protected TimedState[] states;
    }
}