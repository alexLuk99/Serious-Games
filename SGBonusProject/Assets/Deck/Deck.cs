using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new List<Card>();

    void Start()
    {
        InitializeDeck();
    }

    void InitializeDeck()
    {
        // Füge Karten manuell hinzu, du kannst diese später dynamisch aus einem Datenbestand laden
        cards.Add(new Card()); // Null steht für das Bild, das später zugeordnet wird
        cards.Add(new Card());
        cards.Add(new Card());
        cards.Add(new Card());
        cards.Add(new Card());

    }
}
