
using System.IO;
using UnityEngine;

namespace BardicBytes.BardicFramework.Platform
{
    [System.Serializable]
    public struct BuildInfo
    {
        public static BuildInfo LoadDefault()
        {
            var path = Path.Combine(Application.dataPath, "Data");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "buildinfo.json");
            if (!File.Exists(path))
            {
                File.CreateText(path).Dispose();
                var bi = new BuildInfo();
                bi.Update();
                bi.name = Application.productName;
                File.WriteAllText(path, bi.ToJson());
            }
            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<BuildInfo>(json);
        }

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
