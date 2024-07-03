using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    public Button[] levelBtn;
    private int levelSelectionScene = 3;
    // Start is called before the first frame update
    void Start()
    {
        int levelAt = PlayerPrefs.GetInt("levelAt", levelSelectionScene);

        for (int i = 0; i < levelBtn.Length; i++)
        {
            if(i + levelSelectionScene > levelAt)
                levelBtn[i].interactable = false;
        }
        
    }

}
