using Boo.Lang;
using System;
using System.IO;

public class FolderStucture
{
    private DirectoryInfo _parentFolder;
    private List<int> _subfolderIndecies;

    public FolderStucture(DirectoryInfo folder)
    {
        _parentFolder = folder;
        _subfolderIndecies = new List<int>();
    }

    public string GetNextFolder()
    {
        var currentFolder = _parentFolder;
        for (int i = 0; i < _subfolderIndecies.Count; i++)
        {
            var subfolders = currentFolder.GetDirectories();
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

        {
            var subfolders = currentFolder.GetDirectories();
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
}
