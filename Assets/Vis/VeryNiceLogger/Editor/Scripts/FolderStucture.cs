using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class FolderStucture
{
    private readonly DirectoryInfo _parentFolder;
    private readonly List<int> _subfolderIndecies;
    private readonly SpecialFolderType _ignoredSpecialFolders;
    private readonly IList<DefaultAsset> _ignoredFolders;

    public FolderStucture(DirectoryInfo folder, SpecialFolderType ignoredSpecialFolders, IList<DefaultAsset> ignoredFolders)
    {
        _ignoredFolders = ignoredFolders;
        _ignoredSpecialFolders = ignoredSpecialFolders;
        _parentFolder = folder;
        _subfolderIndecies = new List<int>();
    }

    public string GetNextFolder()
    {
        /*
         * Ок, смысл тут в чем. Мы задаем изначальный родительский фолдер Assets
         * 
         * Дальше мы используем _subfolderIndecies для хранения информации о том, где мы в структуре. Это заменяет нам рекурсию. Вместо рекурсии
         * мы просто на каждом цикле заново вспоминаем где мы там находимся.
         * 
         * Если на каком-то уровне у нас индекс превышает кол-во директорий (а такое всегда бывает только на нижнем уровне) - мы удаляем нижний 
         * уровень и возвращаем родителя, плюсую при этом индекс на уровне родителя (при этом, если индексов больше нет - значит мы вышли на Assets, 
         * возвращаем его и это будет последний yield).
         * 
         * Если индекс меньше чем кол-во директорий - просто запоминаем, что мы в этой директории находимся.
         * 
         * Когда мы вспомнили, на чем мы остановились, мы сначала выясняем есть ли в данной папке другие папки. Если есть - надо зайти туда. Запоминаем
         * новый уровень и начальный индекс 0 и рекурсивно вызываем эту же функцию.
         * 
         * Если папок нет, то плюсуем свой уровень и возвращаем самих себя.
         * 
         * При этом нам не надо возвращать игнорируемые папки, поэтому мы в них не заходим и мы их не возвращаем. Т.е. после того как мы вспомнили где мы
         * мы проверяем игнорим ли мы данную папку и если да - вызываем себя рекурсивно (все что надо мы уже приплюсовали).
         * 
         */

        var currentFolder = _parentFolder;
        var subfolders = default(DirectoryInfo[]);
        for (int i = 0; i < _subfolderIndecies.Count; i++)
        {
            subfolders = currentFolder.GetDirectories();
            if (subfolders.Length <= _subfolderIndecies[i])
            {
                _subfolderIndecies.RemoveAt(i);
                if (_subfolderIndecies.Count == 0)
                    return _parentFolder.FullName;
                _subfolderIndecies[_subfolderIndecies.Count - 1]++;
                return currentFolder.FullName;
            }
            else
                currentFolder = subfolders[_subfolderIndecies[i]];
        }

        if (isIgnored(currentFolder))
        {
            _subfolderIndecies[_subfolderIndecies.Count - 1]++;
            if (_subfolderIndecies[_subfolderIndecies.Count - 1] >= subfolders.Length)
            {
                _subfolderIndecies.RemoveAt(_subfolderIndecies.Count - 1);
                if (_subfolderIndecies.Count == 0)
                    return _parentFolder.FullName;
                _subfolderIndecies[_subfolderIndecies.Count - 1]++;
            }
            return GetNextFolder();
        }

        {
            subfolders = currentFolder.GetDirectories();
            if (subfolders.Length > 0)
            {
                _subfolderIndecies.Add(0);
                return GetNextFolder();
            }
            else
                _subfolderIndecies[_subfolderIndecies.Count - 1]++;
        }
        return currentFolder.FullName;
    }

    private bool isIgnored(DirectoryInfo folder)
    {
        switch (folder.Name)
        {
            case "Editor":
                return (_ignoredSpecialFolders & SpecialFolderType.Editor) != 0;
            case "Editor Default Resources":
                return (_ignoredSpecialFolders & SpecialFolderType.EditorDefaultResources) != 0;
            case "Gizmos":
                return (_ignoredSpecialFolders & SpecialFolderType.Gizmos) != 0;
            case "Hidden Assets":
                return (_ignoredSpecialFolders & SpecialFolderType.HiddenAssets) != 0;
            case "Resources":
                return (_ignoredSpecialFolders & SpecialFolderType.Resources) != 0;
            case "Standard Assets":
                return (_ignoredSpecialFolders & SpecialFolderType.StandardAssets) != 0;
            case "StreamingAssets":
                return (_ignoredSpecialFolders & SpecialFolderType.StreamingAssets) != 0;
            case "Plugins":
                return (_ignoredSpecialFolders & SpecialFolderType.Plugins) != 0;
            default:
                var folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folder.FullName.Substring(Application.dataPath.Length - "Assets".Length));
                if (_ignoredFolders.Contains(folderAsset))
                    return true;
                return false;
        }
    }
}
