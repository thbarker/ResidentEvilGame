using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI count;

    // Start is called before the first frame update
    void Awake()
    {
        icon = GetComponent<Image>();
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
        slotMenu = transform.Find("SlotMenu")?.gameObject;
        useButton = slotMenu?.transform.Find("Use")?.gameObject;
        selectButton = transform.Find("SelectButton")?.gameObject;
        combineButton = transform.Find("CombineButton")?.gameObject;
        count = transform.Find("Count")?.GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        slotMenu.SetActive(false);
    }

    public void Use()
    {
        if(item.Use())
            playerInventory.RemoveItemAt(slotNumber-1);
        playerInventory.ChangeState(iStates.Default);
        if (item == null)
        {
            if (playerInventory.itemList.Count > 0)
            {
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(playerInventory.slotList[slotNumber - 2].selectButton);
            } else
            {
                eventSystem.SetSelectedGameObject(null);
            }
        }
    }
    public void BeginCombine()
    {
        if (playerInventory.itemList.Count < 2)
        {
            playerInventory.ChangeState(iStates.Default);
            Debug.Log("Not enough items to combine");
            return;
        }
        bool canCombine = false;
        List<Slot> combineSlots = new List<Slot>();
        foreach (Slot slot in playerInventory.slotList)
        {
            if (slot == this)
                continue;
            if (slot.item != null && item.CanCombine(slot.item))
            {
                combineSlots.Add(slot);
                canCombine = true;
            }
        }
        if (!canCombine)
        {
            playerInventory.ChangeState(iStates.Default);
            Debug.Log("No valid items to combine with");
            return;
        } else
        {
            playerInventory.itemToCombine = item;
            playerInventory.slotToCombine = slotNumber;
            playerInventory.ChangeState(iStates.Combine);
            foreach (Slot slot in combineSlots)
            {
                slot.SetCombineButton(true);
            }
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(combineSlots[0].combineButton);
        }
        // Grey out non combinable slots
        foreach (Slot slot in playerInventory.slotList)
        {
            if(!combineSlots.Contains(slot) && slot.item != null)
                slot.UpdateIcon(0.2f, false);
        } 
    }
    public void Combine()
    {
        Item replacement;
        replacement = item.Combine(playerInventory.itemToCombine);
        if (replacement != null)
        {
            if(playerInventory.slotToCombine < slotNumber)
            {
                playerInventory.selectedSlot = playerInventory.slotList[slotNumber - 2].selectButton;
            } else
            {
                playerInventory.selectedSlot = playerInventory.slotList[slotNumber - 1].selectButton;
            }
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
            playerInventory.SetMessageText("Cannot discard a key item");
            Debug.Log("Cannot Discard a key item");
        } else
        {
            if(item.discardMessage != "")
            {
                playerInventory.SetMessageText(item.discardMessage);
            } else
            {
                playerInventory.SetMessageText(item.name + " discarded");
            }
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
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(useButton);
            playerInventory.selectedSlot = selectButton;
        } 
    }
    
    public void SetSelectButton(bool on)
    {
        selectButton.SetActive(on);
        Debug.Log("Setting" + slotNumber + " To " + on);
    }
    public void SetCombineButton(bool on)
    {
        combineButton.SetActive(on);
    }
    public void SetSlotMenu(bool on)
    {
        slotMenu.SetActive(on);
    }
    public void UpdateIcon(float alpha, bool active)
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            if(item is HandgunBullets)
            {
                HandgunBullets bullets = (HandgunBullets)item;
                count.text = bullets.count.ToString();
                if(bullets.count == bullets.stackSize)
                {
                    count.color = Color.yellow;
                } else
                {
                    count.color = Color.white;
                }
            } else
            {
                count.text = "";
            }
        } else
        {
            icon.sprite = null;
            count.text = "";
        }
        icon.color = new Color(1, 1, 1, alpha);
        selectButton.SetActive(active);
        
    }
}
