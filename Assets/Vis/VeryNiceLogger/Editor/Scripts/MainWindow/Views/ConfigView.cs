using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ConfigView : ViewBase<Config>
{
    private ReorderableList _reorderableList;

    public override void Render(Config config)
    {
        EditorGUILayout.LabelField("Ignored Special Folders:");
        var newSpecialFolders = (SpecialFolderType)EditorGUILayout.EnumFlagsField(config.SpecialFolders);
        if (newSpecialFolders != config.SpecialFolders)
        {
            config.SpecialFolders = newSpecialFolders;
            EditorUtility.SetDirty(config);
        }

        EditorGUILayout.LabelField("Ignored Custom Folders:");
        var serializedConfig = new SerializedObject(config);
        if (_reorderableList == default)
        {
            _reorderableList = new ReorderableList(serializedConfig, serializedConfig.FindProperty("NotVerySpecialFolders"), true, false, true, true);
            _reorderableList.onAddCallback = (ReorderableList list) =>
            {
                config.NotVerySpecialFolders.Add(string.Empty);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _reorderableList.onRemoveCallback = (ReorderableList list) =>
            {
                config.NotVerySpecialFolders.RemoveAt(list.index);
                EditorUtility.SetDirty(config);
                serializedConfig.Update();
            };
            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var newCustomFolderName = EditorGUI.TextField(rect, config.NotVerySpecialFolders[index]);
                if (newCustomFolderName != config.NotVerySpecialFolders[index])
                {
                    config.NotVerySpecialFolders[index] = newCustomFolderName;
                    EditorUtility.SetDirty(config);
                    serializedConfig.Update();
                }
            };
        }
        _reorderableList.DoLayoutList();
        //EditorGUILayout.PropertyField(serializedConfig.FindProperty("NotVerySpecialFolders"), new GUIContent("Custom folders: "), true);
        serializedConfig.ApplyModifiedProperties();
    }
}
