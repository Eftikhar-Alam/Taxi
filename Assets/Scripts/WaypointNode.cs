using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    public List<WaypointNode> connections = new List<WaypointNode>();
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5f);

        foreach (WaypointNode node in connections)
        {
            if (node != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, node.transform.position);
                Vector3 dir = (node.transform.position - transform.position).normalized;
                Gizmos.DrawRay(transform.position, dir * 0.5f);
            }
        }
    }



}
