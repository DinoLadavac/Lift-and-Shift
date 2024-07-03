using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public int nextLevel;
    public ScoreController scoreController;
    public int score;
    public TextMeshProUGUI winText;
    // Start is called before the first frame update

    private void Awake()
    {
        score = PlayerPrefs.GetInt("Score", 0);
    }
    void Start()
    {
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
        winText.gameObject.SetActive(false); // Hide the "WIN" text at the start
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(nextLevel > PlayerPrefs.GetInt("levelAt"))
            {
                PlayerPrefs.SetInt("levelAt", nextLevel);
            }

            score += scoreController.GetScore();
            PlayerPrefs.SetInt("Score", score);
            Time.timeScale = 1f;
            // Show the "WIN" text and start the coroutine to wait for 3 seconds
            StartCoroutine(ShowWinTextAndLoadNextScene());
        }
    }

    private IEnumerator ShowWinTextAndLoadNextScene()
    {
        winText.gameObject.SetActive(true); // Show the "WIN" text
        yield return new WaitForSeconds(3f); // Wait for 3 seconds
        SceneManager.LoadScene(2); // Load the next level
    }

}
