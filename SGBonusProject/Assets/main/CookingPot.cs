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
    private List<Recipe> dishesInPot = new List<Recipe>();
    private Dictionary<Ingredient, Vector2> ingredientPositions = new Dictionary<Ingredient, Vector2>();

        private Dictionary<string, string> transformationMap = new Dictionary<string, string>
    {
        { "Milch", "Sojamilch" },
        { "Käse", "Veganer Käse" },
        { "Butter", "Vegane Butter" },
        { "Honig", "Ahornsirup" },
        { "Maultaschen", "Vegane Maultaschen" },
    };

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
        // Überprüfen, ob die Karte eine Zutat oder ein Gericht ist
        Ingredient ingredient = fridgeController.usedIngredients.Find(i => i.name == card.titleText.text);
        Recipe dish = availableRecipes.Find(r => r.name == card.titleText.text);

        if (ingredient != null)
        {
            ingredientsInPot.Add(ingredient);
            ingredientPositions[ingredient] = card.GetComponent<RectTransform>().anchoredPosition;
            UpdatePotText();
            Debug.Log($"Added {ingredient.name} to the pot.");
            Destroy(card.gameObject);
        }
        else if (dish != null)
        {
            dishesInPot.Add(dish);
            UpdatePotText();
            Debug.Log($"Added {dish.name} to the pot.");
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
        foreach (var dish in dishesInPot)
        {
            potText.text += dish.name + "\n";
        }
        Debug.Log("Zutaten und Gerichte im Topf: " + string.Join(", ", ingredientsInPot.Select(i => i.name).Concat(dishesInPot.Select(d => d.name))));
    }

    public void OnCookButtonClick()
    {
        AudioManager.Instance.PlaySound("click1");

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
        Debug.Log("Current ingredients in pot: " + string.Join(", ", ingredientsInPot.Select(i => i.name).Concat(dishesInPot.Select(d => d.name))));

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

        foreach (var dish in dishesInPot)
        {
            Debug.Log("Checking dish: " + dish.name);
            foreach (var dishIngredient in dish.zutaten)
            {
                if (recipeIngredients.Contains(dishIngredient))
                {
                    recipeIngredients.Remove(dishIngredient);
                    Debug.Log($"Ingredient {dishIngredient} from dish {dish.name} is in the recipe. Remaining: {string.Join(", ", recipeIngredients)}");
                }
                else
                {
                    Debug.Log($"Ingredient {dishIngredient} from dish {dish.name} is not in the recipe.");
                }
            }
        }

        bool isMatch = recipeIngredients.Count == 0;
        Debug.Log("Recipe match result for " + recipe.name + ": " + isMatch);
        return isMatch;
    }

    private void ClearPot()
    {
        ingredientsInPot.Clear();
        dishesInPot.Clear();
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
                cardPositionManager.AssignCardToPosition(cardScript, nextPosition);
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
    public void OnDoubleClick(Card card)
    {
        AudioManager.Instance.PlaySound("click1");
        AudioManager.Instance.PlaySound("click1");

        Ingredient ingredient = fridgeController.usedIngredients.Find(i => i.name == card.titleText.text);

        if (ingredient != null && transformationMap.ContainsKey(ingredient.name))
        {
            string newIngredientName = transformationMap[ingredient.name];
            string newIngredientImage = availableRecipes.Find(r => r.name == newIngredientName)?.bild;

            if (!string.IsNullOrEmpty(newIngredientImage))
            {
                TransformIngredient(card, ingredient, newIngredientName, newIngredientImage);
            }
        }
    }

    private void TransformIngredient(Card card, Ingredient ingredient, string newIngredientName, string newIngredientImage)
    {
        card.SetText(newIngredientName);
        card.SetImage("cardImages/" + newIngredientImage);

        fridgeController.usedIngredients.Remove(ingredient);
        fridgeController.usedIngredients.Add(new Ingredient { name = newIngredientName, bild = newIngredientImage });

        Debug.Log($"{ingredient.name} wurde zu {newIngredientName} transformiert");

        // Optional: Update pot text if the ingredient was already in the pot
        if (ingredientsInPot.Contains(ingredient))
        {
            ingredientsInPot.Remove(ingredient);
            ingredientsInPot.Add(new Ingredient { name = newIngredientName, bild = newIngredientImage });
            UpdatePotText();
        }
    }
}