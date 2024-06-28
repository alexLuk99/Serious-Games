using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;

public class CookingPot : MonoBehaviour
{
    public FridgeController fridgeController;
    public Transform potCardsParent;
    public Text potText;
    public Text resultText;
    public List<Recipe> availableRecipes;
    public CardPositionManager cardPositionManager;

    private List<Ingredient> ingredientsInPot = new List<Ingredient>();
    private Dictionary<Ingredient, Vector2> ingredientPositions = new Dictionary<Ingredient, Vector2>();

    void Start()
    {
        LoadRecipes();
    }

    void LoadRecipes()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("jsonStorage/rezepte");
        if (jsonFile != null)
        {
            RecipeData recipeData = JsonConvert.DeserializeObject<RecipeData>(jsonFile.text);
            availableRecipes = recipeData.gerichte
                .Where(g => g.typ == "gericht")
                .ToList();
            Debug.Log("Loaded recipes: " + string.Join(", ", availableRecipes.Select(r => r.name)));
        }
        else
        {
            Debug.LogError("Cannot find rezepte.json file in Resources!");
        }
    }

    public void OnDropCard(Card card)
    {
        Ingredient ingredient = fridgeController.usedIngredients.Find(i => i.name == card.titleText.text);
        if (ingredient != null)
        {
            ingredientsInPot.Add(ingredient);
            ingredientPositions[ingredient] = card.GetComponent<RectTransform>().anchoredPosition;
            // Die ReleasePosition-Methode wird automatisch aufgerufen, wenn die Karte zerstört wird
            UpdatePotText();
            Debug.Log($"Added {ingredient.name} to the pot.");
            Destroy(card.gameObject);
        }
    }

    private void UpdatePotText()
    {
        potText.text = "Ingredients in Pot:\n";
        foreach (var ingredient in ingredientsInPot)
        {
            potText.text += ingredient.name + "\n";
        }
        Debug.Log("Current ingredients in pot: " + string.Join(", ", ingredientsInPot.Select(i => i.name)));
    }

    public void OnCookButtonClick()
    {
        string cookedDish;
        if (CookRecipe(out cookedDish))
        {
            resultText.text = "Cooked: " + cookedDish;
            Debug.Log("Successfully cooked: " + cookedDish);
            CreateCookedDishCard(cookedDish);
        }
        else
        {
            resultText.text = "Failed to cook anything. Ingredients lost.";
            Debug.Log("Failed to cook anything. Ingredients lost.");
            ClearPot();
        }
    }

    public bool CookRecipe(out string cookedDish)
    {
        cookedDish = null;
        Debug.Log("Starting to check recipes...");

        foreach (var recipe in availableRecipes)
        {
            Debug.Log("Checking recipe: " + recipe.name + " (" + recipe.typ + ")");
            if (recipe.typ == "gericht" && IsRecipeMatch(recipe))
            {
                cookedDish = recipe.name;
                ClearPot();
                return true;
            }
        }
        Debug.Log("No matching recipes found.");
        return false;
    }

    private bool IsRecipeMatch(Recipe recipe)
    {
        List<string> recipeIngredients = new List<string>(recipe.zutaten);
        Debug.Log("Checking if pot ingredients match the recipe: " + recipe.name);
        Debug.Log("Recipe requires ingredients: " + string.Join(", ", recipeIngredients));
        Debug.Log("Current ingredients in pot: " + string.Join(", ", ingredientsInPot.Select(i => i.name)));

        foreach (var ingredient in ingredientsInPot)
        {
            Debug.Log("Checking ingredient: " + ingredient.name);
            if (recipeIngredients.Contains(ingredient.name))
            {
                recipeIngredients.Remove(ingredient.name);
                Debug.Log($"Ingredient {ingredient.name} is in the recipe. Remaining: {string.Join(", ", recipeIngredients)}");
            }
            else
            {
                Debug.Log($"Ingredient {ingredient.name} is not in the recipe.");
            }
        }

        bool isMatch = recipeIngredients.Count == 0;
        Debug.Log("Recipe match result for " + recipe.name + ": " + isMatch);
        return isMatch;
    }

    private void ClearPot()
    {
        ingredientsInPot.Clear();
        ingredientPositions.Clear();
        UpdatePotText();
        Debug.Log("Cleared the pot and released positions.");
    }

    private void CreateCookedDishCard(string cookedDish)
    {
        Recipe recipe = availableRecipes.Find(r => r.name == cookedDish);
        if (recipe != null)
        {
            GameObject card = Instantiate(fridgeController.cardPrefab, potCardsParent);
            Card cardScript = card.GetComponent<Card>();
            cardScript.SetText(recipe.name);
            cardScript.SetImage("cardImages/" + recipe.bild);
            card.GetComponent<RectTransform>().localScale = Vector3.one;

            Vector2 nextPosition = cardPositionManager.GetNextAvailablePosition();
            if (nextPosition != Vector2.zero)
            {
                card.GetComponent<RectTransform>().anchoredPosition = nextPosition;
                Debug.Log($"Created a new card for the cooked dish at position {nextPosition}: {cookedDish}");
            }
            else
            {
                Destroy(card);
                Debug.LogWarning("No available positions for new card.");
            }
        }
        else
        {
            Debug.LogError("Recipe not found for cooked dish: " + cookedDish);
        }
    }
}
