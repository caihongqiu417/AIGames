using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    public BehaviourTree tree;
    public NavMeshAgent agent;
    Node.Status treeStatus = Node.Status.RUNNING;
    
    public enum ActionState { IDLE, WORKING }
    ActionState state = ActionState.IDLE;

    public void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();
    }

    public void Update()
    {
        if (treeStatus != Node.Status.SUCCESS)
        {
            treeStatus = tree.Process();
        }
    }

    public Node.Status GotoLocation(Vector3 location)
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
}