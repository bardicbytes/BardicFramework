//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
#if UNITY_EDITOR
#endif
namespace BardicBytes.BardicFramework.EventVars
{
    public abstract class SimpleGenericEventVar<T> : AutoEvaluatingEventVar<T, T> { }
}