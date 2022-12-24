//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using UnityEngine;

namespace BardicBytes.BardicFramework.EventVars
{
    public abstract class EventVarField
    {
        [HideInInspector]
        public ActorModule module;
        public virtual void Validate()
        {
            Debug.Assert(module != null);
        }
    }
}