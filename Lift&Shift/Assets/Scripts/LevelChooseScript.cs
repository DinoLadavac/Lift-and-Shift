using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChooseScript : MonoBehaviour
{
    public Button[] levelButtons;

    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
                levelButtons[i].interactable = false;
        }
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LevelComplete(int level)
    {
        if (level >= PlayerPrefs.GetInt("LevelReached", 1))
        {
            PlayerPrefs.SetInt("LevelReached", level + 1);
        }
    }
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Level1Scene");
    }
    public void LoadLevel2()
    {
        SceneManager.LoadScene("Level2Scene");
    }
    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level3Scene");
    }
    public void LoadLevel4()
    {
        SceneManager.LoadScene("Level4Scene");
    }
    public void LoadLevel5()
    {
        SceneManager.LoadScene("Level5Scene");
    }
    public void goBack()
    {
        SceneManager.LoadScene("MainMenu");
    }
}