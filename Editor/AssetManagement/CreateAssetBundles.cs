using System.IO;
using UnityEditor;

namespace BardicBytes.BardicFrameworkEditor.AssetManagement
{
    public class CreateAssetBundles
    {
        [MenuItem("Bardic/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                            BuildAssetBundleOptions.None,
                                            BuildTarget.StandaloneWindows);
        }
    }
}