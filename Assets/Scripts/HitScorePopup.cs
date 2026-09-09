using System.Collections;
using TMPro;
using UnityEngine;

public class HitScorePopup : MonoBehaviour
{
    public TMP_Text popupText;
    public float displayDuration = 2f;

    private Coroutine currentRoutine;

    void Start()
    {
        if (popupText != null)
            popupText.gameObject.SetActive(false);
    }

    public void ShowPositive(int points)
    {
        Show($"+{points}", Color.green);
    }

    public void ShowNegative(int points)
    {
        Show($"-{points}", Color.red);
    }

    void Show(string text, Color color)
    {
        if (popupText == null)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        popupText.text = text;
        popupText.color = color;
        popupText.gameObject.SetActive(true);

        currentRoutine = StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);

        if (popupText != null)
            popupText.gameObject.SetActive(false);

        currentRoutine = null;
    }
}
