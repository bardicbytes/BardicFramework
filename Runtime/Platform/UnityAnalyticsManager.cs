//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;
#if BB_ENABLE_ANALYTICS
using UnityEngine.Analytics;
#endif

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Unity Analytics")]
    public class UnityAnalyticsManager : AnalyticsManager
    {
        public override void SendEvent(string eventname)
        {

#if BB_ENABLE_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }

        public override void SendEvent(string eventname, Dictionary<string,object> data)
        {

#if BB_ENABLE_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname, data);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }



        public override void SendEvent(string eventname, Vector3 data)
        {

#if BB_ENABLE_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname, data);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }

    }
}