using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;

    void Start()
    {
        player.transform.position = transform.position;
    }

    void Update()
    {
        player.transform.position = transform.position;
    }
}
