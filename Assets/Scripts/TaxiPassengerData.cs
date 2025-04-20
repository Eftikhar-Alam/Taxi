using UnityEngine;

[CreateAssetMenu(fileName = "TaxiPassengerData", menuName = "GameData/TaxiPassengerData")]
public class TaxiPassengerData : ScriptableObject
{
    [Header("taxi references")]
    public GameObject taxis;

    [Header("passenger references")]
    public GameObject passengers;

   
}
