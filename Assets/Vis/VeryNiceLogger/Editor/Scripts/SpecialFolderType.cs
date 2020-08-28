using System;

[Flags]
public enum SpecialFolderType : byte
{
    Editor = 1,
    EditorDefaultResources = 2,
    Gizmos = 4,
    Resources = 8,
    StandardAssets = 16,
    StreamingAssets = 32,
    HiddenAssets = 64,
    Plugins = 128
}