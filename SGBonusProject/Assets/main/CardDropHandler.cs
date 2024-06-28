using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropHandler : MonoBehaviour, IDropHandler
{
    public enum DropZone
    {
        Pot,
        Trash,
        Customer
    }

    public DropZone dropZone;
    public CookingPot cookingPot;
    public GameState gameState;
    public CardPositionManager cardPositionManager;

    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        if (card != null)
        {
            switch (dropZone)
            {
                case DropZone.Pot:
                    cookingPot.OnDropCard(card);
                    break;
                case DropZone.Trash:
                    cardPositionManager.ReleasePosition(card);
                    Destroy(card.gameObject);
                    break;
                case DropZone.Customer:
                    if (gameState.CheckOrder(card.titleText.text))
                    {
                        cardPositionManager.ReleasePosition(card);
                        Destroy(card.gameObject);
                    }
                    break;
            }
        }
    }
}
