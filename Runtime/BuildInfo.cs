
using System.IO;
using UnityEngine;

namespace Bardic
{
    [System.Serializable]
    public struct BuildInfo
    {
        public static BuildInfo LoadDefault() => JsonUtility.FromJson<BuildInfo>(File.ReadAllText(Path.Combine(Application.dataPath, "Data", "buildinfo.json")));

        public string name;
        public string appVersion;
        public string unityVersion;
        public int build;

        public string BundleVersion => build.ToString();

        public override string ToString()
        {
            return string.Format("ver. {1} {2}", name, appVersion, build);
        }

        public void Update()
        {
            build++;
            this.appVersion = Application.version;
            this.unityVersion = Application.unityVersion;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
