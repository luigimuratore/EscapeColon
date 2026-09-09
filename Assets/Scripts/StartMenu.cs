using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject startMenu;
    public GameObject menuBackground;
    public GameObject minimapUI;

    [Header("Gameplay")]
    public PlayerController playerController;
    public Shooter shooter;
    public CountdownTimer countdownTimer;

    [Header("Audio")]
    public AudioManager audioManager;

    void Start()
    {
        OpenStartMenu();
    }

    void OpenStartMenu()
    {
        Time.timeScale = 0f;

        if (startMenu != null)
            startMenu.SetActive(true);

        if (menuBackground != null)
            menuBackground.SetActive(true);

        if (minimapUI != null)
            minimapUI.SetActive(false);

        if (playerController != null)
            playerController.enabled = false;

        if (shooter != null)
            shooter.enabled = false;

        if (countdownTimer != null)
            countdownTimer.enabled = false;

        if (audioManager != null)
            audioManager.PlayMenuMusic();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        if (startMenu != null)
            startMenu.SetActive(false);

        if (menuBackground != null)
            menuBackground.SetActive(false);

        if (minimapUI != null)
            minimapUI.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        if (shooter != null)
            shooter.enabled = true;

        if (countdownTimer != null)
            countdownTimer.enabled = true;

        if (audioManager != null)
            audioManager.PlayGameplayMusic();

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
