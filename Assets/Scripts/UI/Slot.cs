using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public EventSystem eventSystem;
    public Image icon;
    public Item item;
    public int slotNumber = 1;
    public GameObject slotMenu, useButton, selectButton, combineButton;

    // Start is called before the first frame update
    void Awake()
    {
        icon = GetComponent<Image>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
        slotMenu = transform.Find("SlotMenu")?.gameObject;
        useButton = slotMenu?.transform.Find("UseButton")?.gameObject;
        selectButton = transform.Find("SelectButton")?.gameObject;
        combineButton = transform.Find("CombineButton")?.gameObject;
    }
    private void Start()
    {
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
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0); 
        }
    }

    public void Use()
    {
        item.Use();
        playerInventory.RemoveItemAt(slotNumber-1);
        playerInventory.ChangeState(iStates.Default);
    }
    public void BeginCombine()
    {
        if(playerInventory.itemList.Count < 2)
        {
            playerInventory.ChangeState(iStates.Default);
            Debug.Log("Not enough items to combine");
            return;
        }
        playerInventory.itemToCombine = item;
        playerInventory.slotToCombine = slotNumber;
        playerInventory.ChangeState(iStates.Combine);
        SetCombineButton(false);
    }
    public void Combine()
    {
        Item replacement;
        replacement = item.Combine(playerInventory.itemToCombine);
        if (replacement != null)
        {
            playerInventory.ReplaceItemAt(replacement, slotNumber - 1);
            playerInventory.RemoveItemAt(playerInventory.slotToCombine - 1);
        }
        playerInventory.ChangeState(iStates.Default);
    }
    public void Examine()
    {
        item.Examine();
        playerInventory.ChangeState(iStates.Default);
    }
    public void Discard()
    {
        if(item.isKeyItem)
        {
            Debug.Log("Cannot Discard a key item");
        } else
        {
            Debug.Log(item.name + " Discarded");
            playerInventory.RemoveItemAt(slotNumber - 1);
            playerInventory.selectedSlot = null;
        }
        playerInventory.ChangeState(iStates.Default);
    }

    public void Select() 
    {
        if (item != null)
        {
            playerInventory.ChangeState(iStates.SlotMenu);
            Debug.Log("Setting Slot menu of " + gameObject + "To active");
            slotMenu.SetActive(true);
            //eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(useButton);
            playerInventory.selectedSlot = gameObject;
        } 
    }
    
    public void SetSelectButton(bool on)
    {
        selectButton.SetActive(on);
    }
    public void SetCombineButton(bool on)
    {
        combineButton.SetActive(on);
    }
    public void SetSlotMenu(bool on)
    {
        slotMenu.SetActive(on);
    }
}
