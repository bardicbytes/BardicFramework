//alex@bardicbytes.com
using System.Collections.Generic;
using UnityEngine;
#if ENABLE_CLOUD_SERVICES_ANALYTICS
#endif

namespace BardicBytes.BardicFramework.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Unity Analytics")]
    public class UnityAnalyticsManager : AnalyticsManager
    {
        public override void SendEvent(string eventname)
        {

#if ENABLE_CLOUD_SERVICES_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }

        public override void SendEvent(string eventname, Dictionary<string, object> data)
        {

#if ENABLE_CLOUD_SERVICES_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname, data);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }



        public override void SendEvent(string eventname, Vector3 data)
        {

#if ENABLE_CLOUD_SERVICES_ANALYTICS && !UNITY_EDITOR
            var r = Analytics.CustomEvent(eventname, data);
            if(r != AnalyticsResult.Ok)
            {
                Debug.Log("Analytics Failure: " + r.ToString());
            }
#endif
        }

    }
}