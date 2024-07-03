using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public float startTimerValue = 60f; // Timer starting value
    public float minTimerValueFor1Star = 10f; // Minimum time for 1 star
    public float minTimerValueFor2Stars = 30f; // Minimum time for 2 stars
    public float minTimerValueFor3Stars = 50f; // Minimum time for 3 stars
    public float damage = 2f;

    public RawImage star1; // First star
    public RawImage star2; // Second star
    public RawImage star3; // Third star
    public TextMeshProUGUI timerText; // Timer Text

    private float currentTimerValue;
    private int currentScore;
    private bool isPaused = false;

    private void Start()
    {
        currentTimerValue = startTimerValue;
        currentScore = 3;
        StartCoroutine(TimerCountdown());
    }

    private IEnumerator TimerCountdown()
    {
        while (currentTimerValue > 0)
        {
            if (!isPaused)
            {
                currentTimerValue -= Time.deltaTime;
                timerText.text = Mathf.CeilToInt(currentTimerValue).ToString(); // Display timer without decimals

                if (currentTimerValue < minTimerValueFor1Star && currentScore > 0)
                {
                    UpdateScore(0);
                }
                else if (currentTimerValue < minTimerValueFor2Stars && currentScore > 1)
                {
                    UpdateScore(1);
                }
                else if (currentTimerValue < minTimerValueFor3Stars && currentScore > 2)
                {
                    UpdateScore(2);
                }
            }

            yield return null;
        }

        // Timer reached 0
        timerText.text = "0";
        UpdateScore(0);
    }

    private void UpdateScore(int newScore)
    {
        if (newScore < currentScore)
        {
            currentScore = newScore;
            StartCoroutine(ScaleStar(newScore));
        }
    }

    private IEnumerator ScaleStar(int starIndex)
    {
        RawImage star = null;

        switch (starIndex)
        {
            case 0:
                star = star1;
                break;
            case 1:
                star = star2;
                break;
            case 2:
                star = star3;
                break;
        }

        if (star != null)
        {
            Vector3 originalScale = star.transform.localScale;
            Vector3 targetScale = Vector3.zero;
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                star.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            star.transform.localScale = targetScale;
        }
    }
    public void ApplyDamage()
    {
        currentTimerValue -= damage;
        if (currentTimerValue < 0)
        {
            currentTimerValue = 0;
        }
        timerText.text = Mathf.CeilToInt(currentTimerValue).ToString(); // Update the timer text immediately
    }

    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }

    public int GetScore()
    {
        return currentScore;
    }
}
