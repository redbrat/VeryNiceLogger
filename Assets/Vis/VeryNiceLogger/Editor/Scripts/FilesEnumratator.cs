using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FilesEnumratator
{
    private static FolderStucture _folderStucture;

    public static IEnumerator<string> EnumerateFilesInProject(Config config)
    {
        if (config.IncludedFolders.Count > 0)
        {
            var includedFolders = new List<DefaultAsset>(); //Мы удаляем эти фолдеры по мере прохождения по ним, значит нужно скопировать их, чтобы не потерять их в конфиге
            for (int i = 0; i < config.IncludedFolders.Count; i++)
                includedFolders.Add(config.IncludedFolders[i]);

            while (includedFolders.Count > 0)
            {
                var currentParentFolder = new DirectoryInfo($"{Application.dataPath}{AssetDatabase.GetAssetPath(includedFolders[0]).Substring("Assets".Length)}");
                _folderStucture = new FolderStucture(currentParentFolder, config.SpecialFolders, includedFolders, config.ExcludedFolders);

                var currentFolder = _folderStucture.GetNextFolder();
                while (true)
                {
                    var files = Directory.GetFiles(currentFolder);
                    for (int j = 0; j < files.Length; j++)
                        if (files[j].EndsWith(".cs"))
                            yield return files[j];
                    if (currentFolder == currentParentFolder.FullName)
                        break;
                    currentFolder = _folderStucture.GetNextFolder();
                }
            }
        }
        else
        {
            var dataPathFolder = new DirectoryInfo(Application.dataPath);
            _folderStucture = new FolderStucture(dataPathFolder, config.SpecialFolders, config.IncludedFolders, config.ExcludedFolders);

            var currentFolder = _folderStucture.GetNextFolder();
            while (true)
            {
                var files = Directory.GetFiles(currentFolder);
                for (int i = 0; i < files.Length; i++)
                    if (files[i].EndsWith(".cs"))
                        yield return files[i];
                if (currentFolder == dataPathFolder.FullName)
                    break;
                currentFolder = _folderStucture.GetNextFolder();
            }
        }

    }
}
