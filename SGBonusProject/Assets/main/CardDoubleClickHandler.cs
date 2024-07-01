using UnityEngine;
using UnityEngine.EventSystems;

public class CardDoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
    private float lastClickTime;
    private const float doubleClickTime = 0.3f; // Zeitfenster für Doppelklick
    public CookingPot cookingPot;

    void Start()
    {
        lastClickTime = -doubleClickTime;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - lastClickTime < doubleClickTime)
        {
            Card card = GetComponent<Card>();
            cookingPot.OnDoubleClick(card);
        }
        lastClickTime = Time.time;
    }
}
