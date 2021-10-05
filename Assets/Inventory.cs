using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public Sprite sprite;
    }

    [System.Serializable]
    public class InventorySlotUI
    {
        public Image placeholder;
        public GameObject itemCount;
        public TMPro.TextMeshProUGUI amountText;
    }

    public class Slot
    {
        public InventoryItem item = null;
        public int amount = 0;
    }

    //private bool[] isFull;
    private List<Slot> slots;
    public List<InventoryItem> InventoryItemList;
    public List<InventorySlotUI> InventorySlotUIList;
    public int inventoryMax = 6;

    void Start()
    {
        slots = new List<Slot>(inventoryMax);
        initSlots();
    }

    private void initSlots()
    {
        for(int i = 0; i < inventoryMax; i++)
        {
            slots.Add(new Slot());
        }
        foreach(Slot s in slots)
        {
            Debug.Log(s);
            s.item = null;
            s.amount = 0;
        }
        foreach(InventorySlotUI i in InventorySlotUIList)
        {
            i.itemCount.gameObject.SetActive(false);
            i.amountText.text = "0";
        }
    }

    public InventoryItem GetItem(string name)
    {
        foreach (InventoryItem item in InventoryItemList)
        {
            if (name.ToLower().Contains(item.name.ToLower()))
            {
                return item;
            }
        }
        return null;
    }

    public void AddItem(string name)
    {
        InventoryItem item = GetItem(name);
        Debug.Log(item.name);
        bool added = false;

        if(item != null)
        {
            Debug.Log("Masuk Kondisi");
            Debug.Log(slots.Count);
            for(int i = 0; i < slots.Count; i++)
            {
                Debug.Log("Masuk Loop");
                if(!added)
                {
                    //Debug.Log(item.name);
                    //Debug.Log(slots[i].item);
                    //Kosong

                    if (slots[i].item == null)
                    {
                        Debug.Log("Masuk Null");
                        added = true;
                        slots[i].item = item;
                        InventorySlotUIList[i].placeholder.sprite = item.sprite;
                        InventorySlotUIList[i].itemCount.gameObject.SetActive(true);
                    }
                    if(slots[i].item == item)
                    {
                        added = true;
                        slots[i].amount++;
                        InventorySlotUIList[i].amountText.text = slots[i].amount.ToString();
                    }
                }
            }
        }
        Debug.Log("Selesai Kondisi");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
