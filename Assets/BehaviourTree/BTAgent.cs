using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
    public BehaviourTree tree;
    public NavMeshAgent agent;
    Node.Status treeStatus = Node.Status.RUNNING;
    
    WaitForSeconds waitForSeconds;
    
    public enum ActionState { IDLE, WORKING }
    ActionState state = ActionState.IDLE;

    public void Start()
    {
        
        agent = this.GetComponent<NavMeshAgent>();
        tree = new BehaviourTree();
        waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
        StartCoroutine("Behave");
    }

    IEnumerator Behave()
    {
        while (true)
        {
            treeStatus = tree.Process();
            yield return waitForSeconds;
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
            // 🎯 修复：只在路径确实无效时返回FAILURE
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                state = ActionState.IDLE;
                return Node.Status.FAILURE;
            }
            return Node.Status.RUNNING;
        }
        else if (distance < 2)
        {
            state = ActionState.IDLE;
            agent.ResetPath(); // 🎯 重要：清除导航目标
            return Node.Status.SUCCESS;
        }
        return Node.Status.RUNNING;
    }
}