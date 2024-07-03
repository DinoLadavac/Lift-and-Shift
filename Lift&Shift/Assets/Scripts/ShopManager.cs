using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class ShopManager : MonoBehaviour
{
    public Button blackMagent;
    public GameObject blackMagentObject;
    public TextMeshProUGUI playerStars;
    public int score;

    private int blackMagnet;
    private int blackMagnetBuy;

    private void Awake()
    {
         score = PlayerPrefs.GetInt("Score", 0);
         blackMagnet = 3;
         blackMagnetBuy = PlayerPrefs.GetInt("BlackMagnet", 0);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (score < blackMagnet)
        {
            blackMagent.interactable = false;
        }
        else
        {
            blackMagent.interactable = true;
        }
        playerStars.text = score.ToString();

        if(blackMagnetBuy > 0)
        {
            blackMagentObject.SetActive(false);
        }
        else
        {
            blackMagentObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        playerStars.text = score.ToString();
    }

    private IEnumerator ScaleButton()
    {
        if (blackMagnetBuy == 1)
        {
            Vector3 originalScale = blackMagent.transform.localScale;
            Vector3 targetScale = Vector3.zero;
            float duration = 1f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                blackMagent.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            blackMagent.transform.localScale = targetScale;
        }
    }

    public void Buy()
    {
        blackMagnetBuy = 1;
        score -= 3;
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("BlackMagnet", blackMagnetBuy);
        StartCoroutine(ScaleButton());
    }
}
