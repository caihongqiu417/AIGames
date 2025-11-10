using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : BTAgent
{
    public GameObject diamond;
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
        Selector opendoor = new Selector("opendoor");
        Inverter invertMoney = new Inverter("invertMoney");

        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("HasGotMoney", HasMoney);
        Leaf goToBackdoor = new Leaf("Go to Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go to Frontdoor", GoToFrontdoor);
        Leaf gotoVan = new Leaf("Go to Van", GoToVan);
        
        invertMoney.AddChild(hasGotMoney);
        opendoor.AddChild(goToBackdoor);
        opendoor.AddChild(goToFrontdoor);
        
        steal.AddChild(invertMoney);
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
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
            diamond.SetActive(false);
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
            if (!door.GetComponent<Lock>().isLocked)
            {
                door.SetActive(false);
                return Node.Status.SUCCESS;
            }
            return Node.Status.FAILURE;
        }
        return status;
    }

    public Node.Status GoToDiamond()
    {
        Node.Status status = GotoLocation(diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
            diamond.transform.localPosition = new Vector3(0, 2, 0);
            diamond.transform.localScale = Vector3.one * 0.2f;
        }
        return status;
    }
}