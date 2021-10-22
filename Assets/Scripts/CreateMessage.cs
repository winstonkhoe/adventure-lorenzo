using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateMessage : MonoBehaviour
{

    public GameObject messageObj;
    public TMPro.TextMeshProUGUI messageText;

    void Start()
    {
        messageObj.SetActive(false);
    }

    public void createMessage(string text)
    {
        messageText.text = text;

        StartCoroutine(showMessage(5));
    }
    
    public void createMessage(string text, float time)
    {
        messageText.text = text;

        StartCoroutine(showMessage(time));
    }



    IEnumerator showMessage(float time)
    {
        messageObj.SetActive(true);

        yield return new WaitForSeconds(time);

        clearMessage();

    }



    public void clearMessage()
    {
        messageText.text = "";
        messageObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
