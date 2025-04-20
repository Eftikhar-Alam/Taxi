using System;
using System.Collections;
using UnityEngine;

public class PassengerPickupZone : MonoBehaviour
{
    public bool isPicked = false;
    public bool IsBoarded { get; private set; } = false;

    private Transform targetTaxi;
    private Action onBoardedCallback;
    private Action onDroppedCallback;

    public float walkSpeed = 3f;
    public float reachDistance = 1.5f;
    private bool walkingToTaxi = false;
    private bool droppingOff = false;
    public bool droppingMove;
    private Transform currentFinalTarget = null;
    private bool hasArrivedAtFinalTarget = false;


    public void BeginPickup(Transform carTransform, System.Action onBoardedCallback)
{
    if (isPicked) return;

    isPicked = true;
    StartCoroutine(MoveToCar(carTransform, onBoardedCallback));
}

    private IEnumerator MoveToCar(Transform carTransform, System.Action onBoardedCallback)
    {
        while (Vector3.Distance(transform.position, carTransform.position) > 1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, carTransform.position, Time.deltaTime * walkSpeed);
            yield return null;
        }
       

        IsBoarded = true;
        onBoardedCallback?.Invoke();
        GetComponent<MeshRenderer>().enabled = false;
    }

    void FixedUpdate()
    {
        if (walkingToTaxi && targetTaxi != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTaxi.position, walkSpeed * Time.fixedDeltaTime);
            float dist = Vector3.Distance(transform.position, targetTaxi.position);
            if (dist <= reachDistance)
            {
                IsBoarded = true;
                gameObject.SetActive(false); //
                walkingToTaxi = false;
                onBoardedCallback?.Invoke();
                
            }
        }

        if (droppingOff)
        {
           
            gameObject.SetActive(true);
            droppingOff = false;
            IsBoarded = false;
            onDroppedCallback?.Invoke();
            transform.SetLocalPositionAndRotation(transform.position, Quaternion.identity);
            GetComponent<MeshRenderer>().enabled = true;
            droppingMove = true;
        }
        if (droppingMove)
        {
            MoveToClosestTarget(transform, GameManager.instance.passengersFinalPos, walkSpeed);
          //  Invoke(nameof(stopHimNow), 6f);
            
        }

    }
    void stopHimNow()
    {
        droppingMove = false;
        currentFinalTarget = null;
        hasArrivedAtFinalTarget = false;

        if (targetTaxi != null)
        {
            targetTaxi = null;
        }

        walkingToTaxi = false;
        droppingOff = false;
        IsBoarded = false;
        isPicked = false;
        onDroppedCallback = null;
        onBoardedCallback = null;

        GameManager.instance._completedTrips += 1;
        GameManager.instance.UpdateUi(GameManager.instance.completedTrips, GameManager.instance._completedTrips);
        if (GameManager.instance._numberOfProcessingPassenger > 0)
        {
            GameManager.instance._numberOfProcessingPassenger -= 1;
            GameManager.instance.UpdateUi(GameManager.instance.numberOfProcessingPassenger, GameManager.instance._numberOfProcessingPassenger);
           
        }
    }


    public void MoveToClosestTarget(Transform current, Transform[] targets, float moveSpeed)
    {
        if (targets == null || targets.Length == 0) return;

        // only pck target once
        if (currentFinalTarget == null)
        {
            float shortestDist = Mathf.Infinity;
            foreach (Transform t in targets)
            {
                if (t == null) continue;

                float dist = Vector3.Distance(current.position, t.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    currentFinalTarget = t;
                }
            }
        }

        if (currentFinalTarget != null && !hasArrivedAtFinalTarget)
        {
            float distToTarget = Vector3.Distance(current.position, currentFinalTarget.position);

            if (distToTarget > 0.5f) 
            {
                Vector3 direction = (currentFinalTarget.position - current.position).normalized;
                current.position += direction * moveSpeed * Time.deltaTime;
            }
            else
            {
                hasArrivedAtFinalTarget = true;
                stopHimNow();
            }
        }
    }


    public void BeginDropOff(Action onDropped)
    {
        droppingOff = true;
        onDroppedCallback = onDropped;
    }
}
