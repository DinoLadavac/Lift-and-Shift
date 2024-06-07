using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera carCamera;
    public Camera craneCamera;
    public MonoBehaviour carController;
    public MonoBehaviour craneController;

    private bool  isCarView=false;

    void initialStart()
    {
        SwitchToCarView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCarView = !isCarView;
            if (isCarView)
            {
                SwitchToCarView();
            }
            else
            {
                SwitchToCraneView();
            }
        }
    }

    void SwitchToCarView()
    {
        carCamera.enabled = true;
        craneCamera.enabled = false;
        carController.enabled = true;
        craneController.enabled = false;
    }

    void SwitchToCraneView()
    {
        carCamera.enabled = false;
        craneCamera.enabled = true;
        carController.enabled = false;
        craneController.enabled = true;
    }
}