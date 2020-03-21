using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Preprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        var assetPaths = AssetDatabase.GetAllAssetPaths();
        for (int i = 0; i < assetPaths.Length; i++)
        {
            var path = assetPaths[i];
            if (path.Substring(path.Length - 3) != ".cs")
                continue;
        }
    }
}
