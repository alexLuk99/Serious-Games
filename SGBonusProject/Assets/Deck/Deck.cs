using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

public class Deck : MonoBehaviour
{
    public GameObject Card;
    public List<GameObject> cards = new List<GameObject>();

    void Start(){
        InitializeDeck();
    }

    void InitializeDeck() {
        for (int i = 0; i < 5; i++) {
            GameObject newCard = Instantiate(Card, transform);
            newCard.SetActive(false); // Starte mit deaktivierten Karten
            cards.Add(newCard);
            Debug.Log("Karte erstellt: " + newCard.name);
        }
    }


    public void DisplayCards()
    {
        Debug.Log("Deck wurde geklickt. Karten werden angezeigt.");

        float xOffset = 160f; // Horizontal offset between cards
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject card = cards[i];
            card.SetActive(true); // Activate the card
            RectTransform rect = card.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(xOffset * i, 0); // Position cards horizontally
        }
    }
}
