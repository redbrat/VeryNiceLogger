using System.Collections.Generic;
using UnityEngine;

public class Config : ScriptableObject
{
    public bool LinesLogging;
    public bool ParametersLogging;
    public SpecialFolderType SpecialFolders = (SpecialFolderType)byte.MaxValue;
    public List<string> NotVerySpecialFolders;
}
