using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public EventSystem eventSystem;
    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
    }

    void Start()
    {
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        confirmPickup = GameObject.FindWithTag("Player")?.transform.Find("ConfirmPickupCanvas")?.GetComponent<ConfirmPickup>();
        InitializeItemRef();
    }

    public void Interact()
    {
        if(playerInventory.itemList.Count < playerInventory.slots)
        {
            confirmPickup.itemPickup = this;
            confirmPickup.ShowPickupConfirmation(item.name);
        } else
        {
            Debug.Log("Not enough inventory space.");
        }
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
