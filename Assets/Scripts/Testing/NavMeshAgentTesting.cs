using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentTesting : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    private NavMeshAgent agent;
    private int currentWayPointIndex;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (waypoints.Any())
        {
            agent.SetDestination(waypoints[currentWayPointIndex].position);
        }
    }
    private void Update()
    {
        if (agent.remainingDistance < 0.05f)
        {
            currentWayPointIndex++;
            if (currentWayPointIndex >= waypoints.Length)
            {
                currentWayPointIndex = 0;
                agent.SetDestination(waypoints[currentWayPointIndex].position);
            }
        }
    }
}
