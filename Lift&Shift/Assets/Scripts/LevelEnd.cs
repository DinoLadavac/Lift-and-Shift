using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public int nextLevel;
    public ScoreController scoreController;
    public int score;
    // Start is called before the first frame update

    private void Awake()
    {
        score = PlayerPrefs.GetInt("Score", 0);
    }
    void Start()
    {
        nextLevel = SceneManager.GetActiveScene().buildIndex + 1;
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
            SceneManager.LoadScene(2);
        }
    }

}
