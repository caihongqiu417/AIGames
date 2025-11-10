using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
    public GameObject painting;  // 🎯 新增：画作
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
        DynamicPSelector opendoor = new DynamicPSelector("Dynamic Door");
        Inverter invertMoney = new Inverter("invertMoney");
        PSelector selectLoot = new PSelector("Select Loot");  // 🎯 新增：选择战利品

        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond,1);
        Leaf goToPainting = new Leaf("Go to Painting", GoToPainting,2);  // 🎯 新增：偷画节点
        Leaf hasGotMoney = new Leaf("HasGotMoney", HasMoney);
        Leaf goToBackdoor = new Leaf("Go to Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go to Frontdoor", GoToFrontdoor);
        Leaf gotoVan = new Leaf("Go to Van", GoToVan);
        
        invertMoney.AddChild(hasGotMoney);
        opendoor.AddChild(goToFrontdoor);
        opendoor.AddChild(goToBackdoor);
        
        // 🎯 新增：将钻石和画作都加入选择器
        selectLoot.AddChild(goToDiamond);
        selectLoot.AddChild(goToPainting);
        
        steal.AddChild(invertMoney);
        steal.AddChild(opendoor);
        steal.AddChild(selectLoot);  // 🎯 新增：选择偷窃目标
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
            
            // 🎯 修改：隐藏当前拿着的物品
            if (diamond != null && diamond.transform.parent == this.transform)
            {
                diamond.SetActive(false);
                diamond.transform.parent = null;
                Debug.Log("💰 钻石销赃成功！");
            }
            if (painting != null && painting.transform.parent == this.transform)
            {
                painting.SetActive(false);
                painting.transform.parent = null;
                Debug.Log("💰 画作销赃成功！");
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
        
            // 🎯 修改：如果门没锁，禁用障碍物并返回SUCCESS
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
        
            // 🎯 修改：如果门锁着，返回FAILURE让Selector尝试另一个门
            else
            {
                Debug.Log($"🔒 {door.name} 被锁着，尝试另一个门");
                return Node.Status.FAILURE;
            }
        }
        return status;
    }
    public Node.Status GoToDiamond()
    {
        // 🎯 修改：检查钻石是否可用
        if (diamond == null || !diamond.activeInHierarchy)
            return Node.Status.FAILURE;
            
        Node.Status status = GotoLocation(diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
            diamond.transform.localPosition = new Vector3(0, 2, 0);
            diamond.transform.localScale = Vector3.one * 0.2f;
        }
        return status;
    }

    // 🎯 新增：偷画作的方法
    public Node.Status GoToPainting()
    {
        // 检查画作是否可用
        if (painting == null || !painting.activeInHierarchy)
            return Node.Status.FAILURE;
            
        Node.Status status = GotoLocation(painting.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            painting.transform.parent = this.gameObject.transform;
            painting.transform.localPosition = new Vector3(0, 2, 0);
            painting.transform.localScale = Vector3.one * 0.2f;
        }
        return status;
    }
    
    // 🎯 新增：公共方法用于手动减少金钱
    public void SpendMoney(int amount)
    {
        money = Mathf.Max(0, money - amount);
        Debug.Log($"💸 花费 {amount}，当前金钱: {money}");
    }
}