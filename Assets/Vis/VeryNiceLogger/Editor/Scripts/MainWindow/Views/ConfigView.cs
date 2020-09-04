using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ConfigView : ViewBase<Config>
{
    private ReorderableList _includeReorderableList;
    private ReorderableList _excludeReorderableList;

    public override void Render(Config config)
    {
        var serializedConfig = new SerializedObject(config);
        EditorGUILayout.PropertyField(serializedConfig.FindProperty("LinesLogging"), new GUIContent("Log Lines:"));
        EditorGUILayout.PropertyField(serializedConfig.FindProperty("ParametersLogging"), new GUIContent("Log Function Params:"));

        EditorGUILayout.LabelField("Ignored Special Folders:");
        var newSpecialFolders = (SpecialFolderType)EditorGUILayout.EnumFlagsField(config.SpecialFolders);
        if (newSpecialFolders != config.SpecialFolders)
        {
            config.SpecialFolders = newSpecialFolders;
            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.LabelField("Which folders to include (none means every folder included):");
        if (_includeReorderableList == default)
        {
            _includeReorderableList = new ReorderableList(serializedConfig, serializedConfig.FindProperty("IncludedFolders"), true, false, true, true);
            _includeReorderableList.onAddCallback = (ReorderableList list) =>
            {
                config.IncludedFolders.Add(default);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _includeReorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                config.IncludedFolders.RemoveAt(list.index);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _includeReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var newCustomFolderName = (DefaultAsset)EditorGUI.ObjectField(rect, config.IncludedFolders[index], typeof(DefaultAsset), false);
                if (newCustomFolderName != config.IncludedFolders[index])
                {
                    config.IncludedFolders[index] = newCustomFolderName;
                    EditorUtility.SetDirty(config);
                    serializedConfig.Update();
                }
            };
        }
        _includeReorderableList.DoLayoutList();

        EditorGUILayout.LabelField("Which folders to ignore:");
        if (_excludeReorderableList == default)
        {
            _excludeReorderableList = new ReorderableList(serializedConfig, serializedConfig.FindProperty("ExcludedFolders"), true, false, true, true);
            _excludeReorderableList.onAddCallback = (ReorderableList list) =>
            {
                config.ExcludedFolders.Add(default);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _excludeReorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                config.ExcludedFolders.RemoveAt(list.index);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _excludeReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var newCustomFolderName = (DefaultAsset)EditorGUI.ObjectField(rect, config.ExcludedFolders[index], typeof(DefaultAsset), false);
                if (newCustomFolderName != config.ExcludedFolders[index])
                {
                    config.ExcludedFolders[index] = newCustomFolderName;
                    EditorUtility.SetDirty(config);
                    serializedConfig.Update();
                }
            };
        }
        _excludeReorderableList.DoLayoutList();

        
        //EditorGUILayout.PropertyField(serializedConfig.FindProperty("NotVerySpecialFolders"), new GUIContent("Custom folders: "), true);
        serializedConfig.ApplyModifiedProperties();
    }
}
