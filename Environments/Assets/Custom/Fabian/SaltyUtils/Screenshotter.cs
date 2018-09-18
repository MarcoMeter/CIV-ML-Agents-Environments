using System;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{

    [SerializeField]
    private string screenShotKey = "KeypadDivide";


    void Update()
    {
        if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), screenShotKey, true)))
        {
            ScreenCapture.CaptureScreenshot(DateTime.Now.ToFileTime().ToString() + ".png");
        }
    }
}