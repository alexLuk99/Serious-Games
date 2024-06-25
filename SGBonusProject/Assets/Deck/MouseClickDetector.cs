using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab; // Referenz zum Karten-Prefab
    public Transform spawnPoint; // Punkt, an dem die Karte gespawnt werden soll

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Raycast hit: " + hit.transform.name); // Debugging-Ausgabe
                if (hit.transform.CompareTag("Fridge"))
                {
                    Debug.Log("Fridge clicked"); // Debugging-Ausgabe
                    SpawnCard();
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything"); // Debugging-Ausgabe
            }
        }
    }

    void SpawnCard()
    {
        Debug.Log("Spawning card at: " + spawnPoint.position); // Debugging-Ausgabe
        Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
