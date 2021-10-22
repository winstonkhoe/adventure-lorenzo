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
    public Player player;
    public int inventoryMax = 6;

    void Start()
    {
        player = GetComponent<Player>();
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
        bool added = false;

        if(item != null)
        {
            for(int i = 0; i < slots.Count; i++)
            {
                if(!added)
                {
                    //Kosong
                    if (slots[i].item == null)
                    {
                        //added = true;
                        slots[i].item = item;
                        InventorySlotUIList[i].placeholder.sprite = item.sprite;
                        InventorySlotUIList[i].itemCount.gameObject.SetActive(true);
                    }
                    if(slots[i].item == item)
                    {
                        added = true;
                        slots[i].amount++;
                        InventorySlotUIList[i].amountText.text = slots[i].amount.ToString();
                        break;
                    }
                }
            }
        }
    }

    public void UseItem(int itemSlot)
    {
        itemSlot--;
        Slot use = slots[itemSlot];
        if (use.item != null)
        {
            if(use.item.name.ToLower().Equals("ammo"))
            {
                player.useAmmo();
            }
            else if(use.item.name.ToLower().Equals("healthpotion"))
            {
                player.useHealthPotion();
            }
            else if(use.item.name.ToLower().Equals("skillpotion"))
            {
                player.useSkillPotion();
            }
            else if(use.item.name.ToLower().Equals("shield"))
            {
                player.useShield();
            }
            else if(use.item.name.ToLower().Equals("painkiller"))
            {
                player.usePainKiller();
            }
            else if(use.item.name.ToLower().Equals("damage"))
            {
                player.useDamageMultiplier();
            }
            use.amount--;
            InventorySlotUIList[itemSlot].amountText.text = use.amount.ToString();
            if (use.amount <= 0)
                clearItem(itemSlot);
        }
    }

    public void clearItem(int index)
    {
        InventoryItem defaultItem = GetItem("Default");

        slots[index].item = null;
        slots[index].amount = 0;
        InventorySlotUIList[index].placeholder.sprite = defaultItem.sprite;
        InventorySlotUIList[index].amountText.text = "0";
        InventorySlotUIList[index].itemCount.SetActive(false);

    }

    public void updateSlotInventoryPosition()
    {
        int gapIndex = slots.Count + 1;
        for (int i = 0; i < slots.Count; i++)
        {
            if(slots[i].item == null)
            {
                gapIndex = i;
            }
            else if(slots[i].item != null && i > gapIndex)
            {
                //Change Position
                slots[gapIndex].item = slots[i].item;
                slots[gapIndex].amount = slots[i].amount;
                InventorySlotUIList[gapIndex].amountText.text = slots[gapIndex].amount.ToString();
                InventorySlotUIList[gapIndex].placeholder.sprite = InventorySlotUIList[i].placeholder.sprite;
                InventorySlotUIList[gapIndex].itemCount.gameObject.SetActive(true);

                //Clear Value
                clearItem(i);

                gapIndex = i;
            }
        }
    }

    void Update()
    {
        updateSlotInventoryPosition();
    }
}
