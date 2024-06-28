using UnityEngine;
using UnityEngine.UI;

public class TextBubble : MonoBehaviour
{
    public Text nameText;
    public Text instructionsText;
    public Text orderText;
    public Text timerText;
    public Text punishmentText;

    public void SetText(string name, string instructions, string order, float timer, string punishment)
    {
        nameText.text = $"Name: {name}";
        instructionsText.text = $"Instructions: {instructions}";
        orderText.text = $"Order: {order}";
        timerText.text = $"Timer: {timer:F1}";
        punishmentText.text = $"Punishment: {punishment}";
    }

    public void UpdatePunishmentText(string currentPunishment)
    {
        punishmentText.text = $"Punishment: {currentPunishment}";
    }

    public void UpdateTimer(float timer)
    {
        timerText.text = $"Timer: {timer:F1}";
    }
}
