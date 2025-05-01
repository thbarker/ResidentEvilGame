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
    MansionKey,
    GateKey,
    EmptyFuelCan,
    SiphonPump
}
public class ItemPickup : Interactable
{
    public Items itemEnum;
    public int count;
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

    public override void Interact()
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
                item = new FirstAidSpray(playerInventory);
                break;
            case Items.HandgunBullets:
                item = new HandgunBullets(playerInventory, count);
                break;
            case Items.MansionKey:
                item = new MansionKey(playerInventory, 1);
                break;
            case Items.GateKey:
                item = new GateKey(playerInventory, 1);
                break;
            case Items.EmptyFuelCan:
                item = new EmptyFuelCan(playerInventory);
                break;
            case Items.SiphonPump:
                item = new SiphonPump(playerInventory);
                break;
        }
    }
}
