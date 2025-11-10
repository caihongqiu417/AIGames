using System.Collections.Generic;

public class RandomSelector : Node
{
    // System.Random 是 C# 内置的随机数生成器类，用于生成各种随机数
    private System.Random random;
    
    // 🎯 记录当前选择的节点索引，-1 表示尚未选择或需要重新选择
    private int selectedIndex = -1;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name">选择器名称</param>
    public RandomSelector(string name) : base(name, 0)
    {
        // 创建 Random 实例，用于生成随机数
        random = new System.Random();
    }
    
    /// <summary>
    /// Fisher-Yates 洗牌算法 - 随机打乱子节点顺序
    /// </summary>
    private void ShuffleChildren()
    {
        int n = children.Count;
        while (n > 1)
        {
            n--;
            // random.Next(n + 1) 生成 0 到 n 的随机整数（包含0，不包含n+1）
            int k = random.Next(n + 1);
            
            // 交换两个节点的位置：将随机选中的节点与当前末尾节点交换
            Node value = children[k];
            children[k] = children[n];
            children[n] = value;
        }
    }
    
    /// <summary>
    /// 处理函数：随机选择一个子节点并执行
    /// 关键特性：只在开始时随机选择一次，在执行过程中保持选择的一致性
    /// </summary>
    /// <returns>子节点的执行状态</returns>
    public override Status Process()
    {
        // 🎯 检查是否需要重新选择（第一次执行或上次执行已完成）
        if (selectedIndex == -1)
        {
            // 随机打乱所有子节点顺序
            ShuffleChildren();
            
            // 选择打乱后的第一个节点作为当前执行目标
            selectedIndex = 0;
            ;
        }
        
        // 🎯 执行当前选择的节点
        Status childStatus = children[selectedIndex].Process();
        
        // 🎯 检查节点是否完成执行（成功或失败，但不是运行中）
        if (childStatus != Status.RUNNING)
        {
            // 重置选择索引，下次执行时会重新随机选择
            selectedIndex = -1;
        }
        
        // 🎯 返回子节点的执行状态
        return childStatus;
    }
}