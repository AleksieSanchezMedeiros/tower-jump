using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Text Objects")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text healthText;

    int currentScore = 0;
    public int currentHealth = 3;

    void Start()
    {
        // Initialize UI
        scoreText.text = currentScore.ToString();
        healthText.text = currentHealth.ToString();
    }

    // Call this when score changes
    public void UpdateScore(int addScore)
    {
        scoreText.text = (currentScore += addScore).ToString();
    }

    // Call this when health changes
    public void UpdateHealth(int addHealth)
    {
        healthText.text = (currentHealth += addHealth).ToString();
        if (currentHealth == 0)
        {
            Debug.Log("Player Dead, do something here idk");
            Time.timeScale = 0f; // Pause the game
        }
    }
}
