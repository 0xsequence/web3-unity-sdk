
using UnityEngine;
using UnityEditor;
using System.IO;


//Local build for now
//TODO: download from server
public class CreateAssetBundles
{
#if UNITY_EDITOR
    [MenuItem("Assets/Build Sequence AssetBundles")]
    private static void BuildAllAssetBundles()
    {

        string assetBundleDirectory = Application.dataPath + "/SequenceAssetBundles";

        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
                                        BuildAssetBundleOptions.None,
                                        BuildTarget.StandaloneWindows);


    }
#endif
}
