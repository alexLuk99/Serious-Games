using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Text titleText;
    public Image cardImage;
    public CardPositionManager cardPositionManager;

    private void OnDestroy()
    {
        if (cardPositionManager != null)
        {
            cardPositionManager.ReleasePosition(this);
        }
    }

    private void Start()
    {
        // Wenn cardPositionManager nicht im Inspector gesetzt wurde, versuchen wir, ihn zu finden
        if (cardPositionManager == null)
        {
            cardPositionManager = FindObjectOfType<CardPositionManager>();
        }
    }

    public void SetText(string newText)
    {
        if (titleText != null)
        {
            titleText.text = newText;
        }
    }

    public void SetImage(string imagePath)
    {
        if (cardImage != null)
        {
            Sprite newSprite = Resources.Load<Sprite>(imagePath);
            if (newSprite != null)
            {
                cardImage.sprite = newSprite;
            }
            else
            {
                Debug.LogWarning("Sprite not found at path: " + imagePath);
            }
        }
    }
}
