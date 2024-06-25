using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    PlayerMain  playerMain;
    public int repositoryPosition;
    private string cardName;
    public string imagePath;

    // UI elements
    private GameObject titleObject;
    private Text textComponent;
    private GameObject imageObject;
    private Image imageComponent;
    private RectTransform rectTransform;
    private Canvas canvas;
    private RectTransform canvasRect;

    // Image size variable
    public Vector2 imageSize = new Vector2(100, 100); // Example image size

    // Start is called before the first frame update
    void Start()
    {
        playerMain = FindObjectOfType<PlayerMain>();
        // Initialize cardName and imagePath
        cardName = playerMain.dish[repositoryPosition];
        imagePath = "cardImages/1";  // Example path, adjust as needed

        // Initialize references
        titleObject = this.transform.Find("Card Titel")?.gameObject;
        imageObject = this.transform.Find("Image")?.gameObject;
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        // Set image size
        SetImageSize(imageSize);

        // Set text, image, and position
        SetText(cardName);
        SetImage(imagePath);
        SetPosition(0.1f, 0.1f);  // Example position (10% from left, 10% from bottom)
    }

    // Update is called once per frame
    void Update()
    {
        // Add functionality here if needed
    }

    public void SetText(string newText)
    {
        // Set the text of the card to the newText
        if (titleObject != null)
        {
            textComponent = titleObject.GetComponent<Text>();
            if (textComponent != null)
            {
                textComponent.text = newText;
            }
            else
            {
                Debug.LogWarning("Text component not found on 'Card Titel' object.");
            }
        }
        else
        {
            Debug.LogWarning("'Card Titel' child object not found.");
        }
    }

    public void SetImage(string imagePath)
    {
        // Find the Image component in the child object
        if (imageObject != null)
        {
            imageComponent = imageObject.GetComponent<Image>();
            if (imageComponent != null)
            {
                // Load the sprite from the imagePath
                Sprite newSprite = Resources.Load<Sprite>(imagePath);
                if (newSprite != null)
                {
                    imageComponent.sprite = newSprite;

                    // Ensure the image is fully visible
                    imageComponent.color = Color.white;  // Set the color to white to ensure the image is not tinted
                    imageComponent.material = null;  // Reset material to default
                }
                else
                {
                    Debug.LogWarning("Sprite not found at path: " + imagePath);
                }
            }
            else
            {
                Debug.LogWarning("Image component not found on 'Card Image' object.");
            }
        }
        else
        {
            Debug.LogWarning("'Card Image' child object not found.");
        }
    }

    public void SetImageSize(Vector2 newSize)
    {
        if (imageObject != null)
        {
            RectTransform imageRect = imageObject.GetComponent<RectTransform>();
            if (imageRect != null)
            {
                imageRect.sizeDelta = newSize;
            }
            else
            {
                Debug.LogWarning("RectTransform not found on 'Image' object.");
            }
        }
        else
        {
            Debug.LogWarning("'Image' child object not found.");
        }
    }

    public void SetPosition(float x, float y)
    {
        // Set the position of the card relative to the canvas
        if (rectTransform != null && canvasRect != null)
        {
            // Calculate the position relative to the canvas size
            Vector2 canvasSize = canvasRect.sizeDelta;
            Vector2 newPosition = new Vector2(x * canvasSize.x, y * canvasSize.y);

            // Set the anchored position of the card
            rectTransform.anchoredPosition = newPosition;
        }
        else
        {
            if (rectTransform == null)
                Debug.LogWarning("RectTransform not found on the card.");
            if (canvasRect == null)
                Debug.LogWarning("Canvas RectTransform not found.");
        }
    }
}
