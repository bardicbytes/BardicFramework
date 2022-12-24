//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using System;
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public abstract class GenericEventVarField<InT,OutT, EvT> : EventVarField where EvT : BaseGenericEventVar<InT,OutT,EvT>
    {
        public static implicit operator OutT(GenericEventVarField<InT, OutT, EvT> f) => f.Eval();
        public OutT fallbackValue = default;
        public EvT srcEV = default;

        public Type EventVarType => typeof(EvT);
        public OutT Value => Eval();

        private OutT Eval()
        {
            if (srcEV == null)
            {
                return fallbackValue;
            }
            else if (module == null)
            {

            }
            else if(module.GetModule<EventVarInstancer>().HasInstance(srcEV))
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

        public override void Validate()
        {
            base.Validate();
            Debug.Assert(srcEV != null);
        }
    }
}