using System;
using System.Collections.Generic;

public abstract class RecursiveNodeBase : NodeBase
{
    public readonly List<NodeBase> ChildNodes = new List<NodeBase>();

    internal void Accept(NodeBase childNode)
    {
        ChildNodes.Add(childNode);
    }
}
