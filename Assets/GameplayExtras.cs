using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameplayExtras : MonoBehaviour
{
    //Time
    string fmt = "00";

    private float startTime;
    private float elapsedTime;
    private int minute;
    private int second;

    public TMPro.TextMeshProUGUI timeText;

    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;
        second = (int)elapsedTime % 60;
        minute = (int)((elapsedTime - second) / 60);
        timeText.text = minute.ToString(fmt) + ":" + second.ToString(fmt);
    }

    public float getElapsedTime()
    {
        return elapsedTime;
    }
}
