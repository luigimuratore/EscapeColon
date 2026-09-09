using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 120f;
    public TMP_Text timerText;

    [Header("Lose UI")]
    public GameObject loseText;
    public GameObject restartButton;
    public GameObject saveRunButton;

    [Header("Gameplay")]
    public PlayerController playerController;
    public Shooter shooter;
    public GameManager gameManager;

    [Header("Leaderboard")]
    public LeaderboardManager leaderboardManager;

    [Header("Audio")]
    public AudioManager audioManager;

    private float timeRemaining;
    private bool gameEnded = false;

    void Start()
    {
        timeRemaining = startTime;

        if (loseText != null)
            loseText.SetActive(false);

        if (restartButton != null)
            restartButton.SetActive(false);

        if (saveRunButton != null)
            saveRunButton.SetActive(false);

        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();

        if (leaderboardManager == null)
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();

        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();

        UpdateTimerUI();
    }

    void Update()
    {
        if (gameEnded)
            return;

        // The GameManager handles the win.
        if (FindObjectsByType<Polyp>(FindObjectsSortMode.None).Length == 0)
        {
            gameEnded = true;
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            LoseGame();
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void LoseGame()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (loseText != null)
            loseText.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(true);

        if (saveRunButton != null)
            saveRunButton.SetActive(true);

        if (playerController != null)
            playerController.enabled = false;

        if (shooter != null)
            shooter.enabled = false;

        // Store score + elapsed time, but don't open the name panel yet.
        if (leaderboardManager != null)
        {
            int finalScore = gameManager != null ? gameManager.Score : 0;
            leaderboardManager.PrepareRun(finalScore);
        }

        if (audioManager != null)
            audioManager.PlayLose();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
