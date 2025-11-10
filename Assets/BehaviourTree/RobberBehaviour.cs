using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
    public GameObject[] art;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    [Range(0,1000)] public int money = 800;
    
    private const int MONEY_THRESHOLD = 500;
    private const int ROBBERY_REWARD = 300;

    new void Start()
    {
        base.Start();
        
        Sequence steal = new Sequence("Steal");
        Inverter invertMoney = new Inverter("invertMoney");
        DynamicPSelector opendoor = new DynamicPSelector("Dynamic Door");
        
        // 🎯 主战利品选择器：钻石 vs 随机画作
        PSelector lootPriority = new PSelector("Loot Priority");
        
        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond, 1);
        Leaf hasGotMoney = new Leaf("HasGotMoney", HasMoney);
        Leaf goToBackdoor = new Leaf("Go to Backdoor", GoToBackdoor, 1);
        Leaf goToFrontdoor = new Leaf("Go to Frontdoor", GoToFrontdoor, 1);
        Leaf gotoVan = new Leaf("Go to Van", GoToVan);
        
        // 🎨 创建随机画作选择器（优先级比钻石低）
        RandomSelector randomArt = new RandomSelector("Random Art");
        randomArt.sortOrder = 2; // 画作优先级2
        
        invertMoney.AddChild(hasGotMoney);
        opendoor.AddChild(goToFrontdoor);
        opendoor.AddChild(goToBackdoor);
        
        // 🎨 为所有画作创建节点并添加到随机选择器
        for (int i = 0; i < art.Length; i++)
        {
            int index = i;
            string artName = $"艺术品{i}";
            Leaf goToArt = new Leaf($"Go to Art {i}", () => GoToArtTarget(art[index], artName), 1);
            randomArt.AddChild(goToArt);
        }
        
        // 🎯 主选择器：钻石(优先级1) vs 随机画作(优先级2)
        lootPriority.AddChild(goToDiamond);
        lootPriority.AddChild(randomArt);
        
        steal.AddChild(invertMoney);
        steal.AddChild(opendoor);
        steal.AddChild(lootPriority);
        steal.AddChild(gotoVan);
        
        tree.AddChild(steal);
        tree.PrintTree();
    }

    public Node.Status HasMoney()
    {
        if (money >= MONEY_THRESHOLD)
        {
            return Node.Status.SUCCESS;
        }
        return Node.Status.FAILURE;
    }

    public Node.Status GoToVan()
    {
        Node.Status s = GotoLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += ROBBERY_REWARD;
        
            // 🎨 修改：隐藏当前拿着的所有物品
            if (diamond != null && diamond.transform.parent == this.transform)
            {
                diamond.SetActive(false);
                diamond.transform.parent = null;
                Debug.Log("💰 钻石销赃成功！");
            }
        
            // 🎨 检查 art 数组中的所有画作
            foreach (GameObject artObject in art)
            {
                if (artObject != null && artObject.transform.parent == this.transform)
                {
                    artObject.SetActive(false);
                    artObject.transform.parent = null;
                    Debug.Log($"💰 {artObject.name} 销赃成功！");
                }
            }
        
            Debug.Log($"💰 获得金钱: {ROBBERY_REWARD}，当前金钱: {money}");
        }
        return s;
    }

    public Node.Status GoToBackdoor()
    {
        return GoToDoor(backdoor);
    }

    public Node.Status GoToFrontdoor()
    {
        return GoToDoor(frontdoor);
    }

    public Node.Status GoToDoor(GameObject door)
    {
        Node.Status status = GotoLocation(door.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            Lock doorLock = door.GetComponent<Lock>();
        
            if (doorLock != null && !doorLock.isLocked)
            {
                NavMeshObstacle obstacle = door.GetComponent<NavMeshObstacle>();
                if (obstacle != null)
                {
                    obstacle.carving = false;
                    obstacle.enabled = false;
                }
                Debug.Log($"🚪 {door.name} 已打开");
                return Node.Status.SUCCESS;
            }
            else
            {
                Debug.Log($"🔒 {door.name} 被锁着，尝试另一个门");
                return Node.Status.FAILURE;
            }
        }
        return status;
    }
    
    public Node.Status GoToArtTarget(GameObject artObject, string artName)
    {
        if (artObject == null || !artObject.activeInHierarchy)
        {
            Debug.Log($"🎨 {artName} 不可用");
            return Node.Status.FAILURE;
        }
        
        // 检查是否已经拿着这个画作了
        if (artObject.transform.parent == this.transform)
        {
            Debug.Log($"🎨 已经拿着 {artName} 了");
            return Node.Status.SUCCESS;
        }
        
        Node.Status status = GotoLocation(artObject.transform.position);
        
        // 添加距离检查
        float distance = Vector3.Distance(transform.position, artObject.transform.position);
        Debug.Log($"🎨 前往 {artName} - 状态: {status}, 距离: {distance:F1} 米");
        
        if (status == Node.Status.SUCCESS)
        {
            if (distance <= 2f) // 确保在合理距离内
            {
                artObject.transform.parent = this.gameObject.transform;
                artObject.transform.localPosition = new Vector3(0, 2, 0);
                artObject.transform.localScale = Vector3.one * 0.2f;
                Debug.Log($"🎨 成功偷到 {artName}!");
                return Node.Status.SUCCESS;
            }
            else
            {
                Debug.Log($"🎨 导航完成但距离还有 {distance:F1} 米，继续接近");
                return Node.Status.RUNNING;
            }
        }
        
        return status;
    }
    
    public Node.Status GoToDiamond()
    {
        return GoToArtTarget(diamond, "钻石");
    }
    
    public void SpendMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
        Debug.Log($"💸 花费 {amount}，当前金钱: {money}");
    }
}