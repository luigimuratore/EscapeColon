using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text remainingText;

    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Gameplay")]
    public PlayerController playerController;
    public Shooter shooter;

    [Header("Run")]
    public LeaderboardManager leaderboardManager;

    [Header("Audio")]
    public AudioManager audioManager;

    private int score;
    private int remaining;
    private bool gameWon;

    public int Score => score;
    public int Remaining => remaining;

    void Start()
    {
        remaining = FindObjectsByType<Polyp>(FindObjectsSortMode.None).Length;

        if (winPanel != null)
            winPanel.SetActive(false);

        if (leaderboardManager == null)
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();

        if (shooter == null)
            shooter = FindFirstObjectByType<Shooter>();

        UpdateUI();
    }

    public void PolypDestroyed(int points)
    {
        if (gameWon)
            return;

        score += points;
        remaining = Mathf.Max(0, remaining - 1);

        UpdateUI();

        if (remaining == 0)
            WinGame();
    }

    public void AddScore(int points)
    {
        if (gameWon)
            return;

        score += points;
        UpdateUI();
    }

    void WinGame()
    {
        if (gameWon)
            return;

        gameWon = true;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (playerController != null)
            playerController.enabled = false;

        if (shooter != null)
            shooter.enabled = false;

        if (leaderboardManager != null)
            leaderboardManager.PrepareRun(score);

        if (audioManager != null)
            audioManager.PlayWin();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (remainingText != null)
            remainingText.text = $"Polipi: {remaining}";
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
