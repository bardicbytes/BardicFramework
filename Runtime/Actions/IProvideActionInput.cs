using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Actions
{
    public interface IProvideActionInput
    {
        ActionInputData ActionInputData { get; }

    }
}
