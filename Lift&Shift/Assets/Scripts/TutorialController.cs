using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject carTutorial;
    public GameObject craneTutorial;

    private bool wasT1 = false;
    private bool wasT2 = false;
    private bool wasT3 = false;
    // Start is called before the first frame update
    void Start()
    {
        craneTutorial.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !wasT2)
        {
            carTutorial.SetActive(false);
            craneTutorial.SetActive(true);
            wasT1 = true;
            wasT2 = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && wasT2 && wasT1 && !wasT3)
        {
            carTutorial.SetActive(false);
            craneTutorial.SetActive(true);
            wasT1 = true;
            wasT2 = true;
            wasT3 = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && wasT2 && wasT1 && wasT3)
        {
            carTutorial.SetActive(false);
            craneTutorial.SetActive(false);
            wasT1 = true;
            wasT2 = true;
            wasT3 = true;
        }


    }
}
