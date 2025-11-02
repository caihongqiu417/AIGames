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
    
    public enum ActionState{IDLE,WORKING}
    ActionState state = ActionState.IDLE;
    
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        
        tree = new BehaviourTree();
        Sequence steal = new Sequence("Steal");
        Selector opendoor=new Selector("opendoor");
        
        
        Leaf goToDiamond = new Leaf("Go to Diamond", GoToDiamond);
        Leaf goToBackdoor = new Leaf("Go to Backdoor", GoToBackdoor);
        Leaf goToFrontdoor = new Leaf("Go to Frontdoor", GoToFrontdoor);
        Leaf gotoVan = new Leaf("Go to Van", GoToVan);
        
        
        opendoor.AddChild(goToBackdoor);
        opendoor.AddChild(goToFrontdoor);
        
        steal.AddChild(opendoor);
        steal.AddChild(goToDiamond);
        
        steal.AddChild(gotoVan);
        tree.AddChild(steal);

        tree.PrintTree();
    }
    
    void Update()
    {
        if(treeStatus == Node.Status.RUNNING)
        {
            treeStatus = tree.Process();
        }
        
    }
    
    public Node.Status GoToDiamond()
    {
        return GotoLocation(diamond.transform.position);
    }

    Node.Status GotoLocation(Vector3 location)
    {
        float distance = Vector3.Distance(location,transform.position);
        if (state == ActionState.IDLE)
        {
            agent.SetDestination(location);
            state=ActionState.WORKING;
        }
        else if (Vector3.Distance(agent.pathEndPosition, location) >= 2)
        {
            state=ActionState.IDLE;
            return Node.Status.FAILURE;
        }
        else if (distance < 2)
        {
            state=ActionState.IDLE;
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
    
    
    public Node.Status GoToVan()
    {
        return GotoLocation(van.transform.position);
    }
    
    public Node.Status GoToBackdoor()
    {
        return GotoLocation(backdoor.transform.position);
    }
    
    public Node.Status GoToFrontdoor()
    {
        return GotoLocation(frontdoor.transform.position);
    }
}