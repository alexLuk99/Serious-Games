using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;

public class FridgeController : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handCardsParent;
    public int maxCards = 5;
    public List<Ingredient> availableIngredients;
    public List<Ingredient> usedIngredients;
    public CardPositionManager cardPositionManager;

    void Start()
    {
        LoadIngredients();
        usedIngredients = new List<Ingredient>();
        DrawInitialHand();
    }

    void LoadIngredients()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("jsonStorage/rezepte");
        if (jsonFile != null)
        {
            RecipeData recipeData = JsonConvert.DeserializeObject<RecipeData>(jsonFile.text);
            availableIngredients = recipeData.gerichte
                .Where(g => g.typ == "zutat")
                .Select(g => new Ingredient { name = g.name, bild = g.bild })
                .ToList();
            Debug.Log("Loaded ingredients: " + string.Join(", ", availableIngredients.Select(i => i.name)));
        }
        else
        {
            Debug.LogError("Cannot find rezepte.json file in Resources!");
        }
    }

    public void OnClick()
    {

        if (handCardsParent.childCount < maxCards && availableIngredients.Count > 0 && cardPositionManager.ArePositionsAvailable())
        {
            CreateIngredientCard();
            AudioManager.Instance.PlaySound("rollover1");
        }
        else
        {
            if (!cardPositionManager.ArePositionsAvailable())
            {
                Debug.LogWarning("No available positions for new card.");
            }
            else
            {
                Debug.LogWarning("Max cards reached or no available ingredients.");
            }
        }
    }

    void CreateIngredientCard()
    {
        if (!cardPositionManager.ArePositionsAvailable())
        {
            Debug.LogWarning("No positions available for new card.");
            return;
        }

        Ingredient ingredient = availableIngredients[Random.Range(0, availableIngredients.Count)];
        availableIngredients.Remove(ingredient);
        usedIngredients.Add(ingredient);

        GameObject cardObject = Instantiate(cardPrefab, handCardsParent);
        Card card = cardObject.GetComponent<Card>();
        card.cardPositionManager = this.cardPositionManager;
        card.SetText(ingredient.name);
        card.SetImage("cardImages/" + ingredient.bild);

        RectTransform cardRectTransform = cardObject.GetComponent<RectTransform>();
        cardRectTransform.localScale = Vector3.one;

        Vector2 nextPosition = cardPositionManager.GetNextAvailablePosition();
        if (nextPosition != Vector2.zero)
        {
            cardRectTransform.anchoredPosition = nextPosition;
            cardPositionManager.AssignCardToPosition(card, nextPosition);
            Debug.Log($"Spawned card at position {nextPosition}");
        }
        else
        {
            Destroy(cardObject);
            Debug.LogWarning("No available positions for new card.");
        }
    }

    public void ReleaseCardPosition(Card card)
    {
        cardPositionManager.ReleasePosition(card);
    }

    public void OnPositionReleased()
    {
        Debug.Log("FridgeController notified of released position.");
    }

    void DrawInitialHand()
    {
        for (int i = 0; i < maxCards; i++)
        {
            CreateIngredientCard();
        }
    }
}

public class Ingredient
{
    public string name;
    public string bild;

}

[System.Serializable]
public class Recipe
{
    public string name;
    public List<string> zutaten;
    public string typ;
    public string bild;
    public bool vegan;
}

[System.Serializable]
public class RecipeData
{
    public List<Recipe> gerichte;
}
