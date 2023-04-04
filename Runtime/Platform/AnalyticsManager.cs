using System.Collections.Generic;
using UnityEngine;


namespace BardicBytes.BardicFramework.Platform
{
    public abstract class AnalyticsManager : ScriptableObject
    {
        public abstract void SendEvent(string eventname);
        public abstract void SendEvent(string eventname, Vector3 data);

        public abstract void SendEvent(string eventname, Dictionary<string, object> data);

    }
}