using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobberBehaviour : MonoBehaviour
{
    BehaviourTree tree;
    public GameObject diamond;
    public GameObject van;
    public GameObject backdoor;
    public GameObject frontdoor;
    NavMeshAgent agent;
    Node.Status treeStatus = Node.Status.RUNNING;

    [Range(0,1000)] public int money = 800;

    public enum ActionState
    {
        IDLE,
        WORKING
    }

    ActionState state = ActionState.IDLE;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal");
        Selector opendoor = new Selector("opendoor");


        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf hasGotMoney = new Leaf("HasGotMoney", HasMoney);
        Leaf goToBackdoor = new Leaf("Go to Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go to Frontdoor", GoToFrontdoor);
        Leaf gotoVan = new Leaf("Go to Van", GoToVan);

        
        opendoor.AddChild(goToBackdoor);
        opendoor.AddChild(goToFrontdoor);
        steal.AddChild(hasGotMoney);
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);

        steal.AddChild(gotoVan);
        tree.AddChild(steal);

        tree.PrintTree();
    }

    void Update()
    {
        if (treeStatus !=Node.Status.SUCCESS)
        {
            treeStatus = tree.Process();
        }

    }


    Node.Status GotoLocation(Vector3 location)
    {
        float distance = Vector3.Distance(location, transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(location);
            state = ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, location) >= 2)
        {
            state = ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distance < 2)
        {
            state = ActionState.IDLE;
            return Node.Status.SUCCESS;
        }

        return Node.Status.RUNNING;
    }


    public Node.Status HasMoney()
    {
        if (money >= 500)
        {
            return Node.Status.FAILURE;
        }
        else
        {
            return Node.Status.SUCCESS;
        }
    }
    public Node.Status GoToVan()
    {
        Node.Status s= GotoLocation(van.transform.position);
        if (s == Node.Status.SUCCESS)
        {
            money += 300;
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
        else
        {
            return status;
        }
    }

    public Node.Status GoToDiamond()
    {
        Node.Status status = GotoLocation(diamond.transform.position);
        if (status == Node.Status.SUCCESS)
        {
            diamond.transform.parent = this.gameObject.transform;
            diamond.transform.localPosition = new Vector3(0, 2, 0); // 移到头顶
            diamond.transform.localScale = Vector3.one * 0.2f; // 缩小
        }
        return status;
    }
}