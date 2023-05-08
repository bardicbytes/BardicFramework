using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BardicBytes.DebugUtilities
{
    public class SimpleSpawner : MonoBehaviour
    {
        public GameObject template;
        public float initialDelay = 0f;
        public float interval = .5f;

        private float nextSpawnTime = 0;

        private void Awake()
        {
            nextSpawnTime = Time.time + initialDelay;
        }

        void Update()
        {
            if (Time.time >= nextSpawnTime)
            {
                nextSpawnTime = Time.time + interval;
                Instantiate(template).SetActive(true);
            }
        }
    }
}