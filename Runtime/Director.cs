//alex@bardicbytes.com
//why? https://blog.unity.com/technology/1k-update-calls
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.BardicFramework
{
    public class Director : MonoBehaviour
    {
        private List<Actor> actorList;
        private Actor[] actorArray;

        private bool refreshArray = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
            actorList = new List<Actor>();
            actorArray = new Actor[0];
        }

        private void Update()
        {
            if (refreshArray)
            {
                refreshArray = false;
                actorArray = actorList.ToArray();
            }

            for (int i = 0; i < actorArray.Length; i++)
            {
                actorArray[i].ActorUpdate();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < actorArray.Length; i++)
            {
                actorArray[i].ActorFixedUpdate();
            }
        }

        public void Register(Actor actor)
        {
            Debug.Assert(!actorList.Contains(actor));
            actorList.Add(actor);
            refreshArray = true;
        }

        public void Deregister(Actor actor)
        {
            Debug.Assert(actorList.Contains(actor));
            actorList.Remove(actor);
            refreshArray = true;
        }
    }
}