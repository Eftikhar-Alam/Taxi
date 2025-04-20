
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TaxiPassengerData Data;

    [Range(5,18)] public int totalNumberOfTaxi = 13;
    [Range(5, 30)] public int totalNumberOfPassenger = 10;
    [SerializeField] Transform[] passengersSpawnPos;
    public Transform[] passengersFinalPos;
    [SerializeField] Transform[] TaxiSpawnPos;

    public TextMeshProUGUI totaltaxis, totalpassengers, completedTrips, numberOfProcessingPassenger;
    [HideInInspector]public int _totaltaxis, _totalpassengers, _completedTrips, _numberOfProcessingPassenger;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    void Start()
    {
       Invoke(nameof( InstantiateData),1f);
    }
    public void UpdateUi(TextMeshProUGUI container,int value)
    {
        container.text = value.ToString();
    }
   

    void InstantiateData()
    {
        GameObject taxiParent = new GameObject("Taxis");
        GameObject passengerParent = new GameObject("Passengers");

        for (int i = 0; i < totalNumberOfTaxi; i++)
        {
            GameObject taxiPrefab = Data.taxis;
            GameObject taxi = Instantiate(taxiPrefab, TaxiSpawnPos[i].transform.position, TaxiSpawnPos[i].transform.localRotation, taxiParent.transform);
            AssignRandomColor(taxi);
            _totaltaxis += 1;
            UpdateUi(totaltaxis, _totaltaxis);
          
            
        }
        for (int i = 0; i < totalNumberOfPassenger; i++)
        {
            GameObject passengerPrefab = Data.passengers;
            GameObject passenger = Instantiate(passengerPrefab, passengersSpawnPos[i].transform.position, Quaternion.identity, passengerParent.transform);
            AssignRandomColor(passenger);
            _totalpassengers += 1;
            UpdateUi(totalpassengers, _totalpassengers);
        }
    }

    Vector3 GetRandomPosition()
    {
        return new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));
    }

    void AssignRandomColor(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        Color randomColor;
        do
        {
            randomColor = new Color(Random.value, Random.value, Random.value);
        } while (randomColor == Color.black);

        foreach (Renderer rend in renderers)
        {
            rend.material.color = randomColor;
        }
    }

}
