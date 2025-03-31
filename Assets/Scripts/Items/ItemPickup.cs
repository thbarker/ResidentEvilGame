using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Items
{
    GreenHerb,
    RedHerb,
    FirstAidSpray,
    HandgunBullets,
    Key
}
public class ItemPickup : MonoBehaviour
{
    public Items itemEnum;
    public Item item;
    public PlayerInventory playerInventory;
    public ConfirmPickup confirmPickup;

    void Start()
    {
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        confirmPickup = transform.Find("ConfirmPickupCanvas")?.GetComponent<ConfirmPickup>();
        InitializeItemRef();
    }

    public void Interact()
    {
        if(playerInventory.itemList.Count < playerInventory.slots)
        {
            confirmPickup.ShowPickupConfirmation(item.name);
        } else
        {
            Debug.Log("Not enough inventory space.");
        }
    }

    public void ConfirmPickup()
    {
        confirmPickup.HidePickupConfirmation();
        playerInventory.AddItem(item);
        Destroy(gameObject);

    }
    public void DenyPickup()
    {
        confirmPickup.HidePickupConfirmation();
    }

    public void InitializeItemRef()
    {
        switch (itemEnum)
        {
            case Items.GreenHerb:
                item = new GreenHerb(playerInventory);
                break;
            case Items.RedHerb:
                item = new RedHerb(playerInventory);
                break;
            case Items.FirstAidSpray:
                item = new GreenHerb(playerInventory);
                break;
            case Items.HandgunBullets:
                item = new GreenHerb(playerInventory);
                break;
            case Items.Key:
                item = new GreenHerb(playerInventory);
                break;
        }
    }
}
