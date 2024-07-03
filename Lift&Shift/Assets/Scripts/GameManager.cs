using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Level(int n)
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene(n); // Reload the current scene
    }

    public void QuitGame()
    {
        // Handle quitting the game (e.g., Application.Quit() for standalone builds)
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
