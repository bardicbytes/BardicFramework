//alex@bardicbytes.com
using BardicBytes.BardicFramework.Platform;
using UnityEditor.Build;
using UnityEngine;

namespace BardicBytes.BardicFrameworkEditor.Platform
{
    public class BardicBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {

            var p = System.IO.Path.Combine(Application.dataPath, "Data","buildinfo.json");
            //Debug.Log("loading "+p);
            //TextAsset ta = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Data/buildinfo.txt");
            if (System.IO.File.Exists(p))
            {
                string json = System.IO.File.ReadAllText(p);
                //Debug.Log("loaded "+ta);
                var bi = JsonUtility.FromJson<BuildInfo>(json);
                bi.Update();
                System.IO.File.WriteAllText(System.IO.Path.Combine(Application.dataPath, p), bi.ToJson());
                Debug.Log(bi.ToString()+"\nBardicBuildPreprocessor.OnPreprocessBuild for target " + report.summary.platform + " at path " + report.summary.outputPath);
            }
        }
    }
}