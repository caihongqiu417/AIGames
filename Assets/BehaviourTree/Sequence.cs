using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence(String n)
    {
        name = n;
    }

    public override Status Process()
    {
        Status childstatus = children[currentChild].Process();
        
        if (childstatus == Status.RUNNING) 
            return Status.RUNNING;
            
        if (childstatus == Status.FAILURE) 
        {
            // 🎯 修复：失败时重置计数器
            currentChild = 0;
            return Status.FAILURE;
        }
        
        // SUCCESS 的情况
        currentChild++;
        if (currentChild >= children.Count)
        {
            currentChild = 0;
            return Status.SUCCESS;
        }
        
        return Status.RUNNING;
    }
}