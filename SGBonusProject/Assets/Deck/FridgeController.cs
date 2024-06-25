using UnityEngine;

public class FridgeController : MonoBehaviour
{
    public GameObject Card; // Das Prefab der Karte
    public Transform handCardsParent; // Der Parent für die Handkarten
    public int maxCards = 5; // Maximale Anzahl an Karten in der Hand
    private int currentCardCount = 0; // Aktuelle Anzahl an instanziierten Karten

    void Start()
    {
        // Stelle sicher, dass das GameObject mit dem Tag "Fridge" versehen ist
        if (gameObject.tag != "Fridge")
        {
            Debug.LogError("Das GameObject hat nicht den Tag 'Fridge'.");
        }

        // Erstelle zu Spielbeginn fünf Karten
        for (int i = 0; i < maxCards; i++)
        {
            CreateCard();
        }
    }

    void OnMouseDown()
    {
        // Überprüfe, ob die maximale Anzahl an Karten bereits erreicht ist
        if (currentCardCount < maxCards)
        {
            Debug.Log("Deck wurde geklickt. Karten werden angezeigt.");
            CreateCard();
        }
        else
        {
            Debug.Log("Maximale Anzahl an Karten erreicht.");
        }
    }

    void CreateCard()
    {
        // Erstelle eine neue Karte
        GameObject newCard = Instantiate(Card);
        newCard.transform.SetParent(handCardsParent, false);
        // Setze die Karte als Kind des Handkartenbereichs
        if (handCardsParent != null)
        {
            newCard.transform.SetParent(handCardsParent, false);
            // Stelle sicher, dass die Karte korrekt positioniert wird
            RectTransform rectTransform = newCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Setze die Position der Karte
                rectTransform.anchoredPosition = new Vector2(currentCardCount * 150, 0); // Beispielposition, anpassen nach Bedarf
                rectTransform.localScale = Vector3.one; // Behalte die ursprüngliche Skalierung bei
            }
            else
            {
                Debug.LogError("RectTransform nicht gefunden an der neuen Karte.");
            }
        }
        else
        {
            Debug.LogError("HandCardsParent ist nicht zugewiesen.");
        }

        currentCardCount++;
    }
}
