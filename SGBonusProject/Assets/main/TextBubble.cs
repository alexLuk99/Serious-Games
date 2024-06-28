using UnityEngine;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
    public Text nameText;
    public Text instructionsText;
    public Text orderText;
    public Text timerText;

    public void SetText(string name, string instructions, string order, float timer)
    {
        nameText.text = $"Name: {name}";
        instructionsText.text = $"Instructions: {instructions}";
        orderText.text = $"Order: {order}";
        timerText.text = $"Timer: {timer:F1}";
    }

    public void UpdateTimer(float timer)
    {
        timerText.text = $"Timer: {timer:F1}";
    }
}
