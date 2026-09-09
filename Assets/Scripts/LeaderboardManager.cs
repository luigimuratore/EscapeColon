using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    [Serializable]
    public class RunEntry
    {
        public string playerName;
        public int score;
        public float time;
    }

    [Serializable]
    private class RunEntryList
    {
        public List<RunEntry> entries = new List<RunEntry>();
    }

    [Header("Name Entry UI")]
    public GameObject nameEntryPanel;
    public TMP_InputField nameInput;
    public TMP_Text finalRunText;

    [Header("Leaderboard UI")]
    public GameObject leaderboardPanel;
    public TMP_Text leaderboardText;

    [Header("Delete Entry UI")]
    public TMP_InputField deletePositionInput;
    public TMP_Text deleteFeedbackText;

    [Header("Settings")]
    public int maxEntries = 10;

    private const string SaveKey = "ColonPOV_Leaderboard";

    private List<RunEntry> entries = new List<RunEntry>();

    private float elapsedTime = 0f;
    private bool runFinished = false;

    private int pendingScore = 0;
    private float pendingTime = 0f;
    private bool hasPendingRun = false;

    void Start()
    {
        LoadLeaderboard();

        if (nameEntryPanel != null)
            nameEntryPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (deleteFeedbackText != null)
            deleteFeedbackText.text = "";
    }

    void Update()
    {
        if (!runFinished)
            elapsedTime += Time.deltaTime;
    }

    // Used when the run ends but we do NOT want to open
    // the name-entry panel immediately (for example on loss).
    public void PrepareRun(int score)
    {
        if (runFinished)
            return;

        runFinished = true;
        pendingScore = score;
        pendingTime = elapsedTime;
        hasPendingRun = true;
    }

    // Used when we want to end the run and open the
    // name-entry panel immediately (can still be used if needed).
    public void FinishRun(int score)
    {
        PrepareRun(score);
        OpenNameEntry();
    }

    public void OpenNameEntry()
    {
        if (!hasPendingRun)
            return;

        if (nameEntryPanel != null)
        {
            nameEntryPanel.SetActive(true);
            nameEntryPanel.transform.SetAsLastSibling();
        }

        if (finalRunText != null)
        {
            finalRunText.text =
                $"Punteggio: {pendingScore}\nTempo: {FormatTime(pendingTime)}";
        }

        if (nameInput != null)
        {
            nameInput.text = "";
            nameInput.Select();
            nameInput.ActivateInputField();
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SaveRun()
    {
        if (!hasPendingRun)
            return;

        string playerName = "Player";

        if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.text))
            playerName = nameInput.text.Trim();

        RunEntry newEntry = new RunEntry
        {
            playerName = playerName,
            score = pendingScore,
            time = pendingTime
        };

        entries.Add(newEntry);
        SortLeaderboard();

        if (entries.Count > maxEntries)
            entries.RemoveRange(maxEntries, entries.Count - maxEntries);

        SaveLeaderboard();

        hasPendingRun = false;

        if (nameEntryPanel != null)
            nameEntryPanel.SetActive(false);

        OpenLeaderboard();
    }

    public void OpenLeaderboard()
    {
        LoadLeaderboard();
        RefreshLeaderboardText();

        if (deleteFeedbackText != null)
            deleteFeedbackText.text = "";

        if (deletePositionInput != null)
            deletePositionInput.text = "";

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            leaderboardPanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    // Called by a UI button.
    // Type the leaderboard position (1, 2, 3...) into DeletePositionInput.
    public void DeleteEntryFromInput()
    {
        if (deletePositionInput == null)
        {
            SetDeleteFeedback("Campo posizione non assegnato.");
            return;
        }

        if (!int.TryParse(deletePositionInput.text, out int position))
        {
            SetDeleteFeedback("Inserisci un numero valido.");
            return;
        }

        DeleteEntryByPosition(position);
    }

    // Position is the visible leaderboard position:
    // 1 = first row, 2 = second row, etc.
    public void DeleteEntryByPosition(int position)
    {
        LoadLeaderboard();
        SortLeaderboard();

        int index = position - 1;

        if (index < 0 || index >= entries.Count)
        {
            SetDeleteFeedback("Posizione non valida.");
            return;
        }

        string removedName = entries[index].playerName;

        entries.RemoveAt(index);
        SaveLeaderboard();
        RefreshLeaderboardText();

        if (deletePositionInput != null)
            deletePositionInput.text = "";

        SetDeleteFeedback($"Rimossa posizione {position}: {removedName}");
    }

    public void ClearLeaderboard()
    {
        entries.Clear();

        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        RefreshLeaderboardText();

        if (deletePositionInput != null)
            deletePositionInput.text = "";

        SetDeleteFeedback("Classifica azzerata.");
    }

    private void SetDeleteFeedback(string message)
    {
        if (deleteFeedbackText != null)
            deleteFeedbackText.text = message;
    }

    private void SortLeaderboard()
    {
        entries.Sort((a, b) =>
        {
            int scoreComparison = b.score.CompareTo(a.score);

            if (scoreComparison != 0)
                return scoreComparison;

            return a.time.CompareTo(b.time);
        });
    }

    private void RefreshLeaderboardText()
    {
        if (leaderboardText == null)
            return;

        SortLeaderboard();

        string text =
            "POS   NOME                 PUNTI      TEMPO\n" +
            "------------------------------------------------\n";

        if (entries.Count == 0)
        {
            text += "Nessun risultato salvato.";
        }
        else
        {
            for (int i = 0; i < entries.Count; i++)
            {
                RunEntry entry = entries[i];

                string name = entry.playerName ?? "Player";

                if (name.Length > 18)
                    name = name.Substring(0, 18);

                text +=
                    $"{(i + 1),-5}" +
                    $"{name,-21}" +
                    $"{entry.score,-11}" +
                    $"{FormatTime(entry.time)}\n";
            }
        }

        leaderboardText.text = text;
    }

    private void SaveLeaderboard()
    {
        RunEntryList wrapper = new RunEntryList
        {
            entries = entries
        };

        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    private void LoadLeaderboard()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            entries = new List<RunEntry>();
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
        {
            entries = new List<RunEntry>();
            return;
        }

        RunEntryList wrapper = JsonUtility.FromJson<RunEntryList>(json);

        if (wrapper == null || wrapper.entries == null)
            entries = new List<RunEntry>();
        else
            entries = wrapper.entries;

        SortLeaderboard();
    }

    private string FormatTime(float seconds)
    {
        int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
        int minutes = totalSeconds / 60;
        int secs = totalSeconds % 60;

        return $"{minutes:00}:{secs:00}";
    }
}
