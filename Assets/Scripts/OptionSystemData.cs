using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OptionSystemData
{
    public int display_resolution;
    public float volume;
    public bool isFullScreen;
    public int graphicQualityIndex;
    public OptionSystemData()
    {
        display_resolution = Option.display_resolution;
        volume = Option.volume;
        isFullScreen = Option.isFullScreen;
        graphicQualityIndex = Option.graphicQualityIndex;
    }
}
