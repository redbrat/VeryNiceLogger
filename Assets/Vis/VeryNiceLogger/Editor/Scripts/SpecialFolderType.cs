using System;

[Flags]
public enum SpecialFolderType : byte
{
    Editor = 1,
    EditorDefaultResources = 2,
    Gizmoz = 4,
    Resources = 8,
    StandardAssets = 16,
    StreamingAssets = 32,
    HiddenAssets = 64
}