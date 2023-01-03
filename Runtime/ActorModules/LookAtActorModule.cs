using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework
{
    public class LookAtActorModule : ActorModule
    {
        [SerializeField]
        private ActorTag target = default;
        [SerializeField]
        private bool invert = false;
        [SerializeField]
        private bool keepVertical = false;
        protected override void ActorUpdate()
        {
            if(target == null)
            {
                enabled = false;
                Debug.Log("Disabling LookAtActorModule", this);
                return;
            }


            Vector3 direction = transform.position - target.Actor0.transform.position;
            if (keepVertical)
            {
                direction.y = 0;
            }
            transform.rotation = Quaternion.LookRotation(direction);

            if (invert)
            {
                transform.rotation = Quaternion.LookRotation(-direction);
            }
        }
    }
}


