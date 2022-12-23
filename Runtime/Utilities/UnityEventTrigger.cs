//alex@bardicbytes.com
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BardicBytes.BardicFramework.Utilities
{
    public class UnityEventTrigger : MonoBehaviour
    {
        [SerializeField]
        private float startDelay = default;
        [Space]
        public UnityEvent onAwake;
        public UnityEvent onStart;
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        public UnityEvent onDestroy;

        private void OnValidate()
        {
            if (startDelay < 0) startDelay = 0;
        }

        private void Awake()
        {
            onAwake.Invoke();
        }

        private IEnumerator Start()
        {
            bool noDelay = Mathf.Approximately(startDelay, 0);
            if (noDelay)
                onStart.Invoke();
            yield return new WaitForSeconds(startDelay);
            if (!noDelay)
                onStart.Invoke();
        }

        private void OnEnable()
        {
            onEnable.Invoke();
        }

        private void OnDisable()
        {
            try
            {
                onDisable.Invoke();
            }
            catch (System.Exception)
            {
                Debug.LogError("UnityEventTrigger.OnDisable " + name, this);
                throw;
            }
        }

    }
}
