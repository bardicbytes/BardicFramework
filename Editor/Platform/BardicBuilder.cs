//alex@bardicbytes.com
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Diagnostics;
using System.IO;
using BardicBytes.BardicFramework.Platform;
using BardicBytes.BardicFramework;

namespace BardicBytes.BardicFrameworkEditor.Platform
{
    [CreateAssetMenu(menuName = Prefixes.Platform + "Builder")]
    public class BardicBuilder : ScriptableObject, IBardicEditorable
    {
        public UnityEvent onPreBuild;

        public SceneAsset[] scenes;
        public bool devBuild = false;
        public bool deepProfiling = false;
        public bool scriptDebugging = false;
        public BuildTarget target = BuildTarget.StandaloneWindows;
        public BuildTargetGroup group = BuildTargetGroup.Standalone;
        public string[] defines;
        //public string modifier = "";
        public string exeName = "fileName";
        public string path = "";

        string PathSuffix => target == BuildTarget.WebGL ? "" : string.Format(@"\{0}.exe", exeName);

        public string[] EditorFieldNames => new string[]{"scenes"};
        public bool DrawOtherFields => true;

        //private const string BACKUP = "BackUpThisFolder_ButDontShipItWithYourGame";

        public void SetEnvironment()
        {
            UnityEngine.Debug.Log("Setting Platform Environment "+name);
            System.Text.StringBuilder buildLog = new System.Text.StringBuilder();
            if (string.IsNullOrEmpty(path))
            {
                path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
            }
            BardicBuildPostprocessor.lastBuilderPath = path;
            if (string.IsNullOrEmpty(path)) return;
            onPreBuild.Invoke();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defines);
            UnityEngine.Debug.Log("Platform Environment Set "+name);

        }

        [ContextMenu("Change Environment")]

        public void ChangeEnvironment()
        {
            SetEnvironment();
            EditorUserBuildSettings.SwitchActiveBuildTarget(group,target);
        }

        [ContextMenu("Start Build")]
        public void BuildGame()
        {
            EditorUtility.DisplayProgressBar("Building Game", "Working...", .5f);
            try
            {
                SetEnvironment();

                string[] scenePaths = new string[scenes.Length];
                for (int i = 0; i < scenes.Length; i++)
                    scenePaths[i] = AssetDatabase.GetAssetPath(scenes[i]);

                var report = BuildPipeline.BuildPlayer(scenePaths, path + PathSuffix, target, GetBuildOptions());
                if(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded) OpenFolder();
                var bi = BuildInfo.LoadDefault();
                UnityEngine.Debug.Log("Built " + name + " " + bi + " " + " to " + path);
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            BuildOptions GetBuildOptions()
            {
                BuildOptions bo = BuildOptions.None;
                if (devBuild) bo |= BuildOptions.Development;
                if (devBuild && deepProfiling) bo |= BuildOptions.EnableDeepProfilingSupport;
                if (devBuild && scriptDebugging) bo |= BuildOptions.AllowDebugging;

                return bo;
            }
        }
        

        [ContextMenu("Open Folder")]
        private void OpenFolder()
        {
            if (Directory.Exists(path))
            {
                string p = path.Replace(@"/", @"\");
                var args = string.Format("{1}{0}{1}", p, "\"");
                //UnityEngine.Debug.Log("Opening Explorer: " + args);
                var si = new ProcessStartInfo("explorer.exe", args);
                Process.Start(si);
            }
        }
    }
}