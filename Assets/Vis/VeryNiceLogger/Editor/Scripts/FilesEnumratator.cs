using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FilesEnumratator
{
    private static FolderStucture _folderStucture;

    public static IEnumerator<string> EnumerateFilesInProject(Config config)
    {
        var dataPathFolder = new DirectoryInfo(Application.dataPath);
        _folderStucture = new FolderStucture(dataPathFolder, config.SpecialFolders);

        var currentFolder = _folderStucture.GetNextFolder();
        var appPath = Application.dataPath;
        while (true)
        {
            var files = Directory.GetFiles(currentFolder);
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];

                var directory = Directory.GetParent(file);
                var folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(directory.FullName.Substring(appPath.Length - "Assets".Length));
                if (config.NotVerySpecialFolders.Contains(folderAsset))
                    continue;

                yield return file;
            }
            if (currentFolder == dataPathFolder.FullName)
                break;
            currentFolder = _folderStucture.GetNextFolder();
        }
    }
}
