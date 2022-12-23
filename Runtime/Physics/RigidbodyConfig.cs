using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework.Physics
{
    [System.Serializable]
    public struct RigidbodyConfig
    {
        public bool useGravity, isKinematic;
        public float mass, drag, angularDrag;
    }
}
