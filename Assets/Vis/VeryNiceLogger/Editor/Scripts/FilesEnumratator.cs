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
        _folderStucture = new FolderStucture(dataPathFolder, config.SpecialFolders, config.NotVerySpecialFolders);

        var currentFolder = _folderStucture.GetNextFolder();
        while (true)
        {
            var files = Directory.GetFiles(currentFolder);
            for (int i = 0; i < files.Length; i++)
                yield return files[i];
            if (currentFolder == dataPathFolder.FullName)
                break;
            currentFolder = _folderStucture.GetNextFolder();
        }
    }
}
