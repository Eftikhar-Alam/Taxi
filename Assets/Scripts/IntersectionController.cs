using System.Collections.Generic;
using UnityEngine;

public class IntersectionController : MonoBehaviour
{
    public Queue<TrafficAIController> waitingCars = new Queue<TrafficAIController>();
    private TrafficAIController currentCar = null;
    TrafficAIController nextCar;
    TrafficAIController ai;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Vehicle")) return;

        ai = other.GetComponent<TrafficAIController>();
        if (ai == null) return;

        if (currentCar == null)
        {
            currentCar = ai;
            ai.AllowToProceed();
        }
        else
        {
            if (!waitingCars.Contains(ai))
            {
                waitingCars.Enqueue(ai);
                ai.StopAtIntersection();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Vehicle")) return;

         ai = other.GetComponent<TrafficAIController>();
        if (ai == null || ai != currentCar) return;

        currentCar = null;

        if (waitingCars.Count > 0)
        {
            nextCar = waitingCars.Dequeue();
            currentCar = nextCar;
            currentCar.AllowToProceed();
        }
    }
}
