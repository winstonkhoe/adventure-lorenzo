using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenResolution : MonoBehaviour
{
    private Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    void Awake()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> screenOption = new List<string>();
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            screenOption.Add(option);
        }
        resolutionDropdown.AddOptions(screenOption);
        if(Option.display_resolution == -1)
        {
            Option.display_resolution = resolutions.Length - 1; //Set Maximum Resolution
        }
        resolutionDropdown.value = Option.display_resolution;
        SetResolution(resolutions[Option.display_resolution]);
    }
    
    public void UpdateResolution(int index)
    {
        Debug.Log("Display Resolution: " + index);
        Option.display_resolution = index;
        SetResolution(resolutions[index]);
    }

    private void SetResolution(Resolution res)
    {
        Screen.SetResolution(res.width, res.height, Option.isFullScreen);
    }

    // Update is called once per frame
    void Update()
    {
        resolutionDropdown.value = Option.display_resolution;
    }
}
