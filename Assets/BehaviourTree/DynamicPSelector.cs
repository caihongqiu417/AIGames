using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPSelector : PSelector
{
    public DynamicPSelector(string name) : base(name) { }
    
    public override Status Process()
    {
        OrderNodes(); // 先按当前优先级排序
        
        Status childStatus = children[currentChild].Process();
    
        if (childStatus == Status.RUNNING) 
            return Status.RUNNING;
        
        if (childStatus == Status.SUCCESS) 
        {
            // 🆕 成功：将这个节点移到最前面（最高优先级）
            Node successfulNode = children[currentChild];
            children.RemoveAt(currentChild);
            children.Insert(0, successfulNode);
            successfulNode.sortOrder = 1; // 设为最高优先级
            
            currentChild = 0;
            return Status.SUCCESS;
        }
    
        // 当前子节点失败，尝试下一个
        currentChild++;
        if (currentChild >= children.Count) 
        {
            currentChild = 0;
            return Status.FAILURE;
        }
    
        return Status.RUNNING;
    }
}
