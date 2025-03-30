using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;

    public void UpdateLives(int lives)
    {
        Debug.Log("Updating Lives UI: " + lives);
        livesText.text = "Lives: " + lives.ToString();
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }
}
