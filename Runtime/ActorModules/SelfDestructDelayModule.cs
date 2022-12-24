using BardicBytes.BardicFramework;
using UnityEngine;

namespace BardicBytes.BardicFramework.ActorModules
{
    public class SelfDestructDelayModule : ActorModule
    {
        [field: SerializeField]
        public float Delay { get; protected set; } = 0f;
        [field: SerializeField]
        public bool ScaledTime { get; protected set; } = true;

        private int lastFrame = 0;
        private float lastTime = 0f;

        protected override void OnEnable()
        {
            base.OnEnable();
            Actor.DestructionDelay += DestructingHandler;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private bool DestructingHandler()
        {
            if(Time.frameCount - lastFrame >= 30)
            {
                //been more than a second, its a new destuct
                lastFrame = Time.frameCount;
                lastTime = ScaledTime ? Time.time : Time.unscaledTime;
                return false;
            }

            if(ScaledTime && (Time.time - lastTime < Delay))
            {
                return false;
            }

            return true;

        }
    }

}