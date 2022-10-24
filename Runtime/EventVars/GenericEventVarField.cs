//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using System;
using UnityEngine;

namespace BB.BardicFramework.EventVars
{
    public abstract class BaseEventVarField
    {
        [HideInInspector]
        [SerializeField]
        protected string editorName;
        [Space]
        [HideInInspector]
        public ActorModule module;
        public abstract void Validate(ActorModule actorModule);
    }

    public abstract class GenericEventVarField<InT,OutT, EvT> : BaseEventVarField where EvT : BaseGenericEventVar<InT,OutT,EvT>
    {
        public static implicit operator OutT(GenericEventVarField<InT, OutT, EvT> f) => f.Eval();
        public OutT fallbackValue = default;
        public EvT srcEV = default;

        public Type EventVarType => typeof(EvT);

        private OutT Eval()
        {
            if (srcEV == null)
            {
                return fallbackValue;
            }
            else if (module == null)
            {

            }
            else if(module.Actor.HasActorInstance(srcEV))
            {
                var ai = module.Actor.GetInstance(srcEV);
                if(ai == null && srcEV.RequireInstancing)
                {
                    Debug.LogWarning("failed to find instance for "+srcEV.name+" in "+module.ActorName);
                }
                if (ai == null) return srcEV != null ? srcEV.Value : fallbackValue;
                return ai.Eval<InT, OutT>();
            }

            return srcEV.Value;
        }

        public override void Validate(ActorModule module)
        {
            if (this.module != module) this.module = module;
            if (srcEV == null)
            {
                editorName = "Error. Misconfigured";
                return;
            }
        }
    }
}