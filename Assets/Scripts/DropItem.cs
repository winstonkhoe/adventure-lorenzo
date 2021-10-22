using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{

    public List<GameObject> itemList = new List<GameObject>();

    public GameObject randomItemDrop()
    {
        return itemList[Random.Range(0, itemList.Count - 1)];
    }
}
