using BardicBytes.BardicFramework.Core.EventVars;
using UnityEngine;
using UnityEngine.Events;

namespace BardicBytes.BardicFramework.Core
{
    public class ExceptionManager : MonoBehaviour
    {
        public UnityEvent onException;
        public StringEventVar globalException;
        void Awake()
        {
            Application.logMessageReceived += HandleException;
        }

        void HandleException(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                globalException.Raise(logString+"\n"+stackTrace);
                onException.Invoke();
                Time.timeScale = 0;
            }
        }
    }
}