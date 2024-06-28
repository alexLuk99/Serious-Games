using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CardPositionManager : MonoBehaviour
{
    public Vector2[] cardPositions;
    private Dictionary<Vector2, Card> positionCardMap = new Dictionary<Vector2, Card>();

    void Start()
    {
        InitializePositions();
    }

    void InitializePositions()
    {
        cardPositions = new Vector2[]
        {
            new Vector2(0, -50),
            new Vector2(125, -50),
            new Vector2(250, -50),
            new Vector2(375, -50),
            new Vector2(500, -50)
        };

        foreach (Vector2 position in cardPositions)
        {
            positionCardMap[position] = null;
        }
    }

    public Vector2 GetNextAvailablePosition()
    {
        foreach (var position in cardPositions)
        {
            if (positionCardMap[position] == null)
            {
                return position;
            }
        }
        return Vector2.zero;
    }

    public void AssignCardToPosition(Card card, Vector2 position)
    {
        if (positionCardMap.ContainsKey(position))
        {
            positionCardMap[position] = card;
            Debug.Log($"Card assigned to position {position}");
        }
        else
        {
            Debug.LogWarning($"Position {position} not found in cardPositions");
        }
    }

    public void ReleasePosition(Card card)
    {
        foreach (var kvp in positionCardMap)
        {
            if (kvp.Value == card)
            {
                positionCardMap[kvp.Key] = null;
                Debug.Log($"Position {kvp.Key} has been released");
                return;
            }
        }
        Debug.LogWarning("Card not found in positionCardMap");
    }

    public bool ArePositionsAvailable()
    {
        return positionCardMap.Values.Any(card => card == null);
    }
}
