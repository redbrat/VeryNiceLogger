using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridTests : MonoBehaviour
{
    [ContextMenu("GetMethodRegex")]
    public void GetMethodRegex()
    {
        Debug.Log(RegexPatterns.MethodRegex());
    }
}
