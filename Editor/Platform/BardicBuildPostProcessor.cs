//alex@bardicbytes.com
using UnityEditor.Build;
using UnityEngine;

namespace BardicBytes.BardicFrameworkEditor.Platform
{
    public class BardicBuildPostprocessor : IPostprocessBuildWithReport
    {
        public static string lastBuilderPath = "";
        public int callbackOrder { get { return 0; } }

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < report.GetFiles().Length; i++)
            {
                sb.Append(report.GetFiles()[i].path.Replace(lastBuilderPath, ""));
                if(i != report.GetFiles().Length - 1) sb.Append(", ");
            }
            Debug.Log("Build Complete. "+report.GetFiles().Length+" files. \n"+sb.ToString());
        }
    }
}