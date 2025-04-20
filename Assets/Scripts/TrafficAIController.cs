using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class TrafficAIController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;
    public float stoppingDistance = 3f;
    public LayerMask obstacleLayer;

    private WaypointNode currentNode;
    private WaypointNode lastNode;
    private Rigidbody rb;
    private bool isWaiting;
    private bool isDelivering;
    [SerializeField] PassengerPickupZone currentPassenger;
    private List<WaypointNode> deliveryPath;
    private int deliveryPathIndex = 0;
    Collider[] hits;
    Vector3 direction;
    Vector3 toTarget;
    float maxDistance;
    Quaternion lookRotation;
    PassengerPickupZone pickup;
    float distance;
    WaypointNode dropOffNode;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentNode = TrafficGraphManager.instance.GetNearestNode(transform.position);
        if (currentNode == null || currentNode.connections.Count == 0)
        {
          
            enabled = false;
            return;
        }
        transform.position = currentNode.transform.position;
        PickNextNode();
    }

    public void StopAtIntersection()
    {
       
        isWaiting = true;
    }

    public void AllowToProceed()
    {
       
        isWaiting = false;
    }

    void FixedUpdate()
    {
        if (isWaiting || currentNode == null) return;

        if (currentPassenger == null)
        {
            TryFindNearbyPassenger();
        }

        if (isDelivering && currentPassenger != null && currentPassenger.IsBoarded)
        {
            if (deliveryPathIndex >= deliveryPath.Count)
            {
                isWaiting = true;
                currentPassenger.BeginDropOff(() =>
                {
                    currentPassenger = null;
                    isDelivering = false;
                    isWaiting = true;
                    Debug.Log("destenation reached");
                    Invoke(nameof(letsGoAgain), 1f);
                });
                return;
            }

            currentNode = deliveryPath[deliveryPathIndex];
        }

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, stoppingDistance, obstacleLayer))
        {
            if (hit.collider.CompareTag("Vehicle") || hit.collider.CompareTag("Obstacle"))
            {
                isWaiting = true;
                return;
            }
        }

        MoveTowardsCurrentNode();
    }
    void letsGoAgain()
    {
        isWaiting = false;
       
    }
    void TryFindNearbyPassenger()
    {
        
        if (currentPassenger != null || isDelivering) return;


        hits = Physics.OverlapSphere(transform.position, 15f);
        foreach (Collider h in hits)
        {
            if (h.CompareTag("Passenger"))
            {
                pickup = h.GetComponent<PassengerPickupZone>();
                if (pickup != null && !pickup.isPicked)
                {
                    dropOffNode = FindFarthestNode();
                    deliveryPath = TrafficGraphManager.instance.FindPath(currentNode, dropOffNode);
                    if (deliveryPath == null || deliveryPath.Count < 2)
                    {
                        Debug.LogWarning("Cd nt fnd drop off path.");
                        currentPassenger = null;
                        isWaiting = false;
                        return;
                       // deliveryPath = TrafficGraphManager.instance.FindPath(currentNode, TrafficGraphManager.instance.allNodesk[]);
                    }
                    currentPassenger = pickup;
                    isWaiting = true;

                    currentPassenger.BeginPickup(transform, () =>
                    {

                        currentPassenger.GetComponent<MeshRenderer>().enabled = true;
                        currentPassenger.transform.SetLocalPositionAndRotation(transform.position, Quaternion.identity);
                        deliveryPathIndex = 0;
                        isDelivering = true;
                        isWaiting = false;
                        GameManager.instance._numberOfProcessingPassenger += 1;
                        GameManager.instance.UpdateUi(GameManager.instance.numberOfProcessingPassenger, GameManager.instance._numberOfProcessingPassenger);
                    });
                    break;
                }
            }
        }
    }


    void MoveTowardsCurrentNode()
    {
         toTarget = currentNode.transform.position - transform.position;
         distance = toTarget.magnitude;

        if (distance < 5f)
        {
            transform.position = currentNode.transform.position;
            if (isDelivering) deliveryPathIndex++;
            else PickNextNode();
            return;
        }

        direction = toTarget.normalized;
        lookRotation = Quaternion.LookRotation(direction);
        if (Quaternion.Angle(transform.rotation, lookRotation) > 2f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.fixedDeltaTime * turnSpeed);
        }

        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
    }

    void PickNextNode()
    {
        if (currentNode.connections.Count == 0) return;
        var options = new List<WaypointNode>(currentNode.connections);
        if (lastNode != null && options.Count > 1) options.Remove(lastNode);
        lastNode = currentNode;
        currentNode = options[Random.Range(0, options.Count)];
    }

    WaypointNode FindFarthestNode()
    {
         maxDistance = 0f;
        WaypointNode farthest = currentNode;
        foreach (var node in TrafficGraphManager.instance.allNodes)
        {
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if (dist > maxDistance-4f)
            {
                maxDistance = dist;
                farthest = node;
            }
        }
        return farthest;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            
            isWaiting = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
           
            isWaiting = true;
          
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
           
            isWaiting = false;
        }
    }
}
