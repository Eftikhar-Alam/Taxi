using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    public List<WaypointNode> connections = new List<WaypointNode>();
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 5f);

        foreach (WaypointNode nod in connections)
        {
            if (nod != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nod.transform.position);
                Vector3 dir = (nod.transform.position - transform.position).normalized;
                Gizmos.DrawRay(transform.position, dir * 0.5f);
            }
        }
    }



}
