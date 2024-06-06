using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("LevelChoose");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
