using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Option : MonoBehaviour
{
    public GameObject OptionMenu;
    void Awake()
    {
        OptionMenu.SetActive(false);
        LoadSystemData();    
        initResolutionDropDown();
    }

    void Start()
    {
        //LoadSystemData();
        volumeSlider.value = volume;
        resolutionDropdown.value = display_resolution;
        fullScreen.isOn = isFullScreen;
        Screen.fullScreen = isFullScreen;
        GraphicDropDown.value = graphicQualityIndex;
    }

    void Update()
    {
        //LoadSystemData();
        volumeSlider.value = volume;
        resolutionDropdown.value = display_resolution;
        fullScreen.isOn = isFullScreen;
        GraphicDropDown.value = graphicQualityIndex;
    }

    #region Screen Resolution Code

    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;
    public static int display_resolution = -1;

    private void SetResolution(Resolution res)
    {
        Screen.SetResolution(res.width, res.height, isFullScreen);
        SaveSystemData();
    }

    void initResolutionDropDown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> screenOption = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (Option.display_resolution == -1)
            {
                if (Screen.currentResolution.width == resolutions[i].width && Screen.currentResolution.height == resolutions[i].height)
                {
                    Option.display_resolution = i; //Set Based on current Resolution
                }
            }
            string option = resolutions[i].width + " x " + resolutions[i].height;
            screenOption.Add(option);
        }
        resolutionDropdown.AddOptions(screenOption);

        resolutionDropdown.value = Option.display_resolution;
        SetResolution(resolutions[Option.display_resolution]);
    }

    public void UpdateResolution(int index)
    {
        display_resolution = index;
        SetResolution(resolutions[index]);
        SaveSystemData();
    }
    #endregion

    #region Full Screen Code
    
    public Toggle fullScreen;
    public static bool isFullScreen = false;

    public void updateFullScreen(bool toggle)
    {
        isFullScreen = toggle;
        Screen.fullScreen = isFullScreen;
        SaveSystemData();
    }

    #endregion

    #region Graphic Quality Code

    public TMP_Dropdown GraphicDropDown;
    public static int graphicQualityIndex;

    public void setQualityLevel(int qualityIndex)
    {
        graphicQualityIndex = qualityIndex;
        QualitySettings.SetQualityLevel(qualityIndex);
        SaveSystemData();
    }

    #endregion

    #region Volume Code

    public Slider volumeSlider;
    public static float volume = 0.5f;

    public void updateVolume(float volume)
    {
        Option.volume = volume;
        SaveSystemData();
    }

    #endregion

    #region SaveSystem

    public static void SaveSystemData()
    {
        Debug.Log("Save Data");
        SaveSystem.SaveSystemData();
    }

    public static void LoadSystemData()
    {
        OptionSystemData data = SaveSystem.LoadSystemData();

        if (data != null)
        {
            isFullScreen = data.isFullScreen;
            display_resolution = data.display_resolution;
            volume = data.volume;
            graphicQualityIndex = data.graphicQualityIndex;
        }
    }

    #endregion



}
