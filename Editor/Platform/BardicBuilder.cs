//alex@bardicbytes.com
using UnityEditor;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using System.IO;

namespace BB.BardicFramework.Platform.Editor
{
    [CreateAssetMenu(menuName = Prefixes.Platform+"Builder")]
    public class BardicBuilder : ScriptableObject
    {
        public UnityEvent onPreBuild;

        public SceneAsset[] scenes;
        public bool devBuild = false;
        public bool deepProfiling = false;
        public bool scriptDebugging = false;
        public BuildTarget target = BuildTarget.StandaloneWindows;
        public BuildTargetGroup group = BuildTargetGroup.Standalone;
        public string[] defines;
        public string modifier = "";
        public string path = "";

        //private const string BACKUP = "BackUpThisFolder_ButDontShipItWithYourGame";

        public void SetEnvironment()
        {
            UnityEngine.Debug.Log("Setting Platform Environment "+name);
            System.Text.StringBuilder buildLog = new System.Text.StringBuilder();
            if (string.IsNullOrEmpty(path))
            {
                path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
            }
            BardicBuildPostprocessor.path = path;
            if (string.IsNullOrEmpty(path)) return;
            //UnityEngine.Debug.Log(name+" onPreBuild.Invoke()?");
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
            SetEnvironment();

            string[] scenePaths = new string[scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
                scenePaths[i] = AssetDatabase.GetAssetPath(scenes[i]);

            var pathSuffix = target == BuildTarget.WebGL ? "" : @"\warDrive.exe";
            var report = BuildPipeline.BuildPlayer(scenePaths, path + pathSuffix, target, GetBuildOptions());
                      
            
            //Compress(GetFiles(path), path + @"\" + "warDrive" + modifier + ".zip");

            OpenFolder();
            var bi = BuildInfo.LoadDefault();
            EditorUtility.ClearProgressBar();
            UnityEngine.Debug.Log("Built " + name +" "+bi+" " +" to " + path);

            BuildOptions GetBuildOptions()
            {
                BuildOptions bo = BuildOptions.None;
                if (devBuild) bo |= BuildOptions.Development;
                if (devBuild && deepProfiling) bo |= BuildOptions.EnableDeepProfilingSupport;
                if (devBuild && scriptDebugging) bo |= BuildOptions.AllowDebugging;

                return bo;
            }

            //void Compress(IEnumerable<string> filePaths, string zipFileOutputPath)
            //{
            //    List<byte[]> buffers = new List<byte[]>();
            //    List<string> paths = new List<string>();

            //    foreach (var f in filePaths)
            //    {
            //        if (f.Contains(BACKUP))
            //        {
            //            buildLog.AppendLine("skipping backup file " + f);
            //            continue;
            //        }

            //        if (File.Exists(f))
            //        {
            //            buffers.Add(File.ReadAllBytes(f));
            //            paths.Add(f);
            //        }
            //        else if (Directory.Exists(f))
            //        {
            //            paths.Add(f);
            //            buffers.Add(null);
            //        }
            //    }
            //    buildLog.AppendLine("zipping " + zipFileOutputPath);

            //    using (var zipToOpen = new FileStream(zipFileOutputPath, FileMode.CreateNew))
            //    {
            //        using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            //        {
            //            for(int i =0; i < buffers.Count; i++)
            //            {
            //                var e = paths[i].Replace(path+@"\", "") + (buffers[i] == null ? @"\" : "");
            //                var zipArchiveEntry = archive.CreateEntry(e);
            //                using (var zipStream = zipArchiveEntry.Open())
            //                {
            //                    if (buffers[i] == null)
            //                    {
            //                        buildLog.AppendLine("dir\t\t" + zipArchiveEntry);
            //                    }
            //                    else
            //                    {
            //                        buildLog.AppendLine(buffers[i].Length+ " bytes\t\t" + zipArchiveEntry);
            //                        zipStream.Write(buffers[i], 0, buffers[i].Length);
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    File.WriteAllText(Path.Combine(path, "build.log"), buildLog.ToString());
            //}

            ////https://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
            //IEnumerable<string> GetFiles(string path)
            //{
            //    Queue<string> queue = new Queue<string>();
            //    queue.Enqueue(path);
            //    while (queue.Count > 0)
            //    {
            //        path = queue.Dequeue();
            //        try
            //        {
            //            foreach (string subDir in Directory.GetDirectories(path))
            //            {
            //                queue.Enqueue(subDir);
            //            }
            //        }
            //        catch (System.Exception ex)
            //        {
            //            UnityEngine.Debug.LogWarning(ex);
            //        }
            //        IEnumerable<string> files = null;
            //        try
            //        {
            //            files = Directory.GetFiles(path).Concat(Directory.GetDirectories(path));

            //        }
            //        catch (System.Exception ex)
            //        {
            //            UnityEngine.Debug.LogWarning(ex);
            //        }
            //        if (files != null)
            //        {
            //            var e = files.GetEnumerator();
            //            while (e.MoveNext())
            //            {
            //                yield return e.Current;
            //            }
            //        }
            //    }
            //}
        }

        

        [ContextMenu("Open Folder")]
        private void OpenFolder()
        {
            if(Directory.Exists(path))
                Process.Start(new ProcessStartInfo("explorer.exe", path));
        }
    }
}