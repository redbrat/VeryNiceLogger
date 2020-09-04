using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Config : ScriptableObject
{
    public bool LinesLogging;
    public bool ParametersLogging;
    public SpecialFolderType SpecialFolders = (SpecialFolderType)byte.MaxValue;
    public List<DefaultAsset> ExcludedFolders;
    public List<DefaultAsset> IncludedFolders;
}
