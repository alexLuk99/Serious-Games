using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    private int score = 100;

    public void UpdateScore(int amount)
    {
        score += amount;
        scoreText.text = $"Score: {score}";
    }

    public int GetScore()
    {
        return score;
    }
}
