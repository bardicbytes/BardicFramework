//alex@bardicbytes.com
using BardicBytes.BardicFramework.EventVars;
using UnityEngine;
using UnityEngine.Events;

namespace BardicBytes.BardicFramework
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