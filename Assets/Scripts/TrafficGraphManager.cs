using System.Collections.Generic;
using UnityEngine;

public class TrafficGraphManager : MonoBehaviour
{
    public static TrafficGraphManager instance;

    public List<WaypointNode> allNodes = new List<WaypointNode>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

       // allNodes.AddRange(FindObjectsOfType<WaypointNode>());
    }

    public WaypointNode GetNearestNode(Vector3 position)
    {
        float minDist = Mathf.Infinity;
        WaypointNode nearest = null;

        foreach (WaypointNode node in allNodes)
        {
            float dist = Vector3.Distance(position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = node;
            }
        }
       

        
        return nearest;
    }

    public List<WaypointNode> FindPath(WaypointNode start, WaypointNode goal)
    {
        if (start == null || goal == null)
        {
            Debug.LogError("Start or goal is null.");
            return new List<WaypointNode>();
        }

        var openSet = new PriorityQueue<WaypointNode>();
        var cameFrom = new Dictionary<WaypointNode, WaypointNode>();
        var gScore = new Dictionary<WaypointNode, float>();
        var fScore = new Dictionary<WaypointNode, float>();

        WaypointNode bestReached = start;
        float bestHeuristic = Heuristic(start, goal);

        openSet.Enqueue(start, 0);
        gScore[start] = 0;
        fScore[start] = bestHeuristic;

        while (openSet.Count > 0)
        {
            WaypointNode current = openSet.Dequeue();

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            float currentHeuristic = Heuristic(current, goal);
            if (currentHeuristic < bestHeuristic)
            {
                bestReached = current;
                bestHeuristic = currentHeuristic;
            }

            foreach (var neighbor in current.connections)
            {
                if (neighbor == null) continue;

                float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }

        Debug.LogWarning($"A* failed to find full path. Returning path to best node: {bestReached.name}");
        return ReconstructPath(cameFrom, bestReached);
    }


    float Heuristic(WaypointNode a, WaypointNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    List<WaypointNode> ReconstructPath(Dictionary<WaypointNode, WaypointNode> cameFrom, WaypointNode current)
    {
        List<WaypointNode> path = new List<WaypointNode>();
        while (current != null)
        {
            path.Insert(0, current);
            cameFrom.TryGetValue(current, out current);
        }
        return path;
    }

}
