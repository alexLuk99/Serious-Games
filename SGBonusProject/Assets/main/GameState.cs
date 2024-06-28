using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public List<Customer> customers;
    public GameObject customerPrefab;
    public GameObject textBubblePanel;
    public ScoreManager scoreManager;
    public FridgeController fridgeController;
    public CookingPot cookingPot;
    public Text punishmentText;

    private Customer currentCustomer;
    private float currentTimer;
    private bool orderCompleted;
    private TextBubble textBubble;
    private int currentPunishment;

    void Start()
    {
        textBubble = textBubblePanel.GetComponent<TextBubble>();
        LoadCustomersFromJson();
        StartCoroutine(SpawnCustomers());
    }

    void Update()
    {
        if (currentCustomer != null)
        {
            currentTimer -= Time.deltaTime;
            textBubble.UpdateTimer(currentTimer);

            if (orderCompleted)
            {
                CompleteOrder();
            }
            else if (currentTimer <= 0)
            {
                FailOrder();
            }
        }
    }

    void LoadCustomersFromJson()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("jsonStorage/customers");
        if (jsonFile != null)
        {
            string dataAsJson = jsonFile.text;
            CustomerList loadedData = JsonConvert.DeserializeObject<CustomerList>(dataAsJson);
            customers = new List<Customer>(loadedData.customers);
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
        yield return new WaitForSeconds(1);

        if (customers.Count > 0)
        {
            currentCustomer = customers[0];
            currentTimer = currentCustomer.timer;
            currentPunishment = currentCustomer.punishment;
            UpdatePunishmentText();
            customers.RemoveAt(0);
            textBubble.SetText(currentCustomer.name, currentCustomer.instructions, currentCustomer.order, currentTimer, currentPunishment.ToString());
        }
    }

    public void CompleteOrder()
    {
        scoreManager.UpdateScore(1);
        StartCoroutine(SpawnNextCustomer());
        currentCustomer = null;
        orderCompleted = false;
    }

    public void FailOrder()
    {
        scoreManager.UpdateScore(-currentPunishment);
        StartCoroutine(SpawnNextCustomer());
        currentCustomer = null;
    }

    public bool CheckOrder(string dish)
    {
        if (currentCustomer != null && dish == currentCustomer.order)
        {
            Debug.Log("Order completed!");
            orderCompleted = true;
            currentPunishment = 0;
            UpdatePunishmentText();
            return true;
        }
        else if (currentCustomer != null)

        {
            Debug.Log("Less punishment");
            currentPunishment = Mathf.Max(currentPunishment - 2, 0);
            UpdatePunishmentText();
            return true; // Rückgabewert ändern, damit die Karte entfernt wird
        }
        return false;
    }

    private bool IsIngredient(string dish)
    {
        return fridgeController.availableIngredients.Exists(ingredient => ingredient.name == dish);
    }

    private void UpdatePunishmentText()
    {
        punishmentText.text = $"Punishment: {currentPunishment}";
        textBubble.UpdatePunishmentText(currentPunishment.ToString());
    }
}

public class Customer
{
    public string name;
    public string instructions;
    public string order;
    public float timer;
    public int punishment;
}

public class CustomerList
{
    public List<Customer> customers;
}
