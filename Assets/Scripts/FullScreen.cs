using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreen : MonoBehaviour
{
    public Toggle fullScreen;

    void Start()
    {
        fullScreen.isOn = Option.isFullScreen;
    }

    public void ToggleFullScreen()
    {
        Screen.fullScreen = Option.isFullScreen;
    }

    public void updateFullScreen(bool toggle)
    {
        Option.isFullScreen = toggle;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
