using UnityEngine;
using UnityEngine.AI;

public class NavMeshDestinationVisualizer : MonoBehaviour
{
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.hasPath)
        {
            Debug.DrawLine(transform.position, agent.destination, Color.red);
        }
    }
}
