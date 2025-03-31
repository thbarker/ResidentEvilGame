using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public Image icon;
    public Item item;
    public GameObject slotMenu;

    // Start is called before the first frame update
    void Start()
    {
        icon = GetComponent<Image>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        slotMenu = transform.Find("SlotMenu").gameObject;
        slotMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            icon.color = new Color(1, 1, 1, 1);
        } else
        {
            icon.color = new Color(1, 1, 1, 0);
        }
    }

    public void Use()
    {
        item.Use();
        playerInventory.RemoveItem(item);
        slotMenu.SetActive(false);
    }
    public void Combine()
    {
        slotMenu.SetActive(false);
    }
    public void Examine()
    {
        item.Examine();
        slotMenu.SetActive(false);
    }
    public void Discard()
    {
        playerInventory.RemoveItem(item);
        slotMenu.SetActive(false);
    }

    public void Select() 
    {
        if(item != null)
            slotMenu.SetActive(true);   
    }
}
