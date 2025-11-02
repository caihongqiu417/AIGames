using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    public Selector(String n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childStatus = children[currentChild].Process();
    
        if (childStatus == Status.RUNNING) 
            return Status.RUNNING;
        
        if (childStatus == Status.SUCCESS) 
        {
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
