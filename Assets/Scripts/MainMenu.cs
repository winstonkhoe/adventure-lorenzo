using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<AudioManager>().Play("MenuSong");
        //FindObjectOfType<AudioManager>().Play("MenuSong");
    }

    public void PlayGame()
    {
        FindObjectOfType<AudioManager>().InterceptSong("PinkSoldier");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
