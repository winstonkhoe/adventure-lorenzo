using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    void Start()
    {
        volumeSlider.value = Option.volume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
