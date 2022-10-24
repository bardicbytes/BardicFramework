//alex@bardicbytes.com
//why? https://www.youtube.com/watch?v=raQ3iHhE_Kk
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BardicBytes.BardicFramework.Core.EventVars
{
    public abstract class GenericMinMaxEventVar<T> : GenericEventVar<T>
    {
        [Header("MinMax")]
        [SerializeField]
        protected bool hasMin = false;
        [SerializeField]
        protected T minValue;
        [SerializeField]
        protected bool hasMax = false;
        [SerializeField]
        protected T maxValue;

        public T MinValue
        {
            get => minValue;
            set => minValue = value;
        }
        public T MaxValue
        {
            get => maxValue;
            set => maxValue = value;
        }

        public abstract T MinMaxClamp(T val);

        public override void Raise(T data)
        {
            base.Raise(MinMaxClamp(data));
        }
    }
}