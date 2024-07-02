using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public ScoreController scoreController; // Reference to the ScoreController

    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        scoreController.SetPaused(false); // Unpause the timer
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        scoreController.SetPaused(true); // Pause the timer
    }

    public void Restart()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }

    public void QuitGame()
    {
        // Handle quitting the game (e.g., Application.Quit() for standalone builds)
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
