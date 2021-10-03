using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Option : MonoBehaviour
{
    public static int display_resolution = -1;
    public static float volume = 0.5f;
    public static bool isFullScreen = false;
    

    void Awake()
    {
        LoadSystemData();    
    }

    public void SaveSystemData()
    {
        SaveSystem.SaveSystemData();
    }

    public static float getVolume()
    {
        return Option.volume;
    }

    public void LoadSystemData()
    {
        OptionSystemData data = SaveSystem.LoadSystemData();

        if(data != null)
        {
            isFullScreen = data.isFullScreen;
            display_resolution = data.display_resolution;
            volume = data.volume;
        }
    }

    public void updateVolume(float volume)
    {
        Option.volume = volume;
    }

}
