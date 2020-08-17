using UnityEngine;
public class CurlyBracketsNode : RecursiveNodeBase
{
    public bool IsBlocksContainer
    {
        get
        {
            if (ParentNode == default) //Значит у нас блок первого уровня, т.е. просто файл, а там не может сразу быть инструкций, тут не питон.
                return false;
            if (ParentNode.ChildNodes.Count == 1) //Блок просто для логического отделения
                return true;
            var lastNode = ParentNode.ChildNodes[ParentNode.ChildNodes.Count - 2];
            if (lastNode is RoundBracketsNode)
            {
                var parametersToFunction = (lastNode as RoundBracketsNode);
                var precedingExpression = (lastNode as RecursiveNodeBase).PrecedingExpression;
                Debug.Log($"Function: precedingExpression - {precedingExpression}, parametersToFunction({parametersToFunction.ChildNodes.Count})");
            }
            else if (lastNode is CommandNode)
            {
                var precedingExpression = PrecedingExpression.Trim();
                if (precedingExpression.Length > 0)
                    Debug.Log($"Property: precedingExpression = {precedingExpression}");
                //else
                //    Debug.Log($"Property: precedingExpression = {precedingExpression}");
            }
            return true;
            //RegexPatterns.IsBlocksContainer(PrecedingExpression);
            //throw new NotImplementedException();
        }
    }
}
