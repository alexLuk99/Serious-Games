using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameState : MonoBehaviour
{
    public List<Customer> customers;
    public int totalScore = 0;
    public GameObject customerPrefab; // Referenz zum HumanMale_Character_FREE Prefab
    public GameObject textBubblePanel; // Referenz zur TextBubblePanel

    private Customer currentCustomer;
    private float currentTimer;
    private bool orderCompleted;
    private TextBubble textBubble;

    // Start is called before the first frame update
    void Start()
    {
        textBubble = textBubblePanel.GetComponent<TextBubble>();
        LoadCustomersFromJson();
        StartCoroutine(SpawnCustomers());
    }

    // Update is called once per frame
    void Update()
    {
        if (currentCustomer != null)
        {
            currentTimer -= Time.deltaTime;

            // Update Timer in TextBubble
            if (textBubble != null)
            {
                textBubble.timerText.text = $"Timer: {currentTimer:F1}";
            }

            // Überprüfen, ob die Bestellung erledigt ist
            if (orderCompleted)
            {
                currentCustomer = null;
                orderCompleted = false;
                totalScore += 1; // Punkte hinzufügen für eine erfüllte Bestellung
                StartCoroutine(SpawnNextCustomer());
            }
            else if (currentTimer <= 0)
            {
                currentCustomer = null;
                totalScore -= 1; // Punkteabzug für nicht erfüllte Bestellung
                StartCoroutine(SpawnNextCustomer());
            }
        }
    }

    void LoadCustomersFromJson()
    {
        // Laden der Datei als TextAsset aus dem Resources-Ordner
        TextAsset jsonFile = Resources.Load<TextAsset>("jsonStorage/customers");

        if (jsonFile != null)
        {
            string dataAsJson = jsonFile.text;
            CustomerList loadedData = JsonConvert.DeserializeObject<CustomerList>(dataAsJson);
            customers = new List<Customer>(loadedData.customers);

            // Log-Ausgabe der Kunden
            foreach (var customer in customers)
            {
                Debug.Log($"Name: {customer.name}, Instructions: {customer.instructions}, Order: {customer.order}, Timer: {customer.timer}");
            }
        }
        else
        {
            Debug.LogError("Cannot find customers file in Resources!");
        }
    }

    IEnumerator SpawnCustomers()
    {
        yield return SpawnNextCustomer();
    }

    IEnumerator SpawnNextCustomer()
    {
        yield return new WaitForSeconds(1); // Optional: Wartezeit zwischen Kunden

        if (customers.Count > 0)
        {
            currentCustomer = customers[0];
            currentTimer = currentCustomer.timer;
            customers.RemoveAt(0);

            // Aktualisieren der TextBubble mit neuen Kundendaten
            if (textBubble != null)
            {
                textBubble.SetText(currentCustomer.name, currentCustomer.instructions, currentCustomer.order, currentTimer);
            }
        }
    }

    public void CompleteOrder()
    {
        orderCompleted = true;
    }
}

[System.Serializable]
public class Customer
{
    public string name;
    public string instructions;
    public string order;
    public float timer;
}

[System.Serializable]
public class CustomerList
{
    public Customer[] customers;
}
