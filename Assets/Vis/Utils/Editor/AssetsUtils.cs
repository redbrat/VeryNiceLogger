using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Vis.Utils
{
    public static class AssetsUtils
    {
        public static T GetScriptableObjectFromDB<T>(string dbPointerName, string assetName) where T : ScriptableObject
        {
            var dbPointerGuids = AssetDatabase.FindAssets(dbPointerName);
            if (dbPointerGuids.Length == 0)
                throw new ApplicationException($"[{nameof(AssetsUtils)}] Asset installation corrupted. Try reimport asset from AssetStore!");
            var dbPointerPath = AssetDatabase.GUIDToAssetPath(dbPointerGuids[0]);
            var slicingSettingsPath = Path.Combine(dbPointerPath.Substring(0, dbPointerPath.Length - Path.GetFileName(dbPointerPath).Length), assetName);
            var instance = AssetDatabase.LoadAssetAtPath<T>(slicingSettingsPath);
            if (instance == null)
            {
                instance = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(instance, slicingSettingsPath);
                AssetDatabase.SaveAssets();
                instance = AssetDatabase.LoadAssetAtPath<T>(slicingSettingsPath);
            }
            return instance;
        }
    }
}
