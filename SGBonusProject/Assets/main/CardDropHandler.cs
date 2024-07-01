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
    public FridgeController fridgeController; // Referenz zum FridgeController


    public void OnDrop(PointerEventData eventData)
    {
        Card card = eventData.pointerDrag.GetComponent<Card>();

        if (card != null)
        {
            switch (dropZone)
            {
                case DropZone.Pot:
                    cookingPot.OnDropCard(card);
                    AudioManager.Instance.PlaySound("switch2");
                    break;
                case DropZone.Trash:
                    cardPositionManager.ReleasePosition(card);
                    AudioManager.Instance.PlaySound("switch2");
                    Destroy(card.gameObject);
                    fridgeController.CreateIngredientCard();
                    break;
                case DropZone.Customer:
                Debug.Log("Checking order for " + card.titleText.text);
                    if (gameState.CheckOrder(card.titleText.text))
                    {
                        cardPositionManager.ReleasePosition(card);
                        AudioManager.Instance.PlaySound("switch2");
                        Destroy(card.gameObject);
                    }
                    break;
            }
        }
    }
}
