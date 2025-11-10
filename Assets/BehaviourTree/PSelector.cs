

using System;

public class PSelector : Node
{
    public PSelector(String n)
    {
        name = n;
    }

    //-----------------sort---------------------
    protected void OrderNodes()
    {
        // 将子节点列表转换为数组进行排序
        Node[] childArray = children.ToArray();
        Sort(childArray, 0, childArray.Length - 1);
        
        // 将排序后的数组重新放回children列表
        children.Clear();
        children.AddRange(childArray);
    }
    
    int Partition(Node[] array, int low, int high)
    {
        // 1. 选择基准元素（选最后一个），这就是第一个"坑"
        Node pivot = array[high];
    
        // 2. 记录当前坑的位置，开始是high位置
        int hole = high;
    
        // 3. 从左向右找大数填右边的坑，从右向左找小数填左边的坑
        int left = low;
        int right = high - 1;
    
        while (left <= right)
        {
            // 从左往右找比基准大的数（要扔到右边）
            while (left <= right && array[left].sortOrder <= pivot.sortOrder)
            {
                left++;
            }
            // 找到大的了，把它填到右边的坑里
            if (left <= right)
            {
                array[hole] = array[left];
                hole = left;  // 坑移到左边
            }
        
            // 从右往左找比基准小的数（要扔到左边）
            while (left <= right && array[right].sortOrder >= pivot.sortOrder)
            {
                right--;
            }
            // 找到小的了，把它填到左边的坑里
            if (left <= right)
            {
                array[hole] = array[right];
                hole = right;  // 坑移到右边
            }
        }
    
        // 4. 最后把基准填到最后的坑里
        array[hole] = pivot;
    
        // 5. 返回基准的最终位置
        return hole;
    }

    void Sort(Node[] array, int low, int high)
    {
        if (low < high)
        {
            // 分区操作，获取基准位置
            int pivotIndex = Partition(array, low, high);
            
            // 递归排序左半部分
            Sort(array, low, pivotIndex - 1);
            
            // 递归排序右半部分
            Sort(array, pivotIndex + 1, high);
        }
    }
    //------------------sort---------------------------------
    
    public override Status Process()
    {
        OrderNodes();
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