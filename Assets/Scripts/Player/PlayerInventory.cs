using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum iStates
{
    Default,
    SlotMenu,
    Combine
}
public class PlayerInventory : MonoBehaviour
{
    public List<Item> itemList;
    public List<Slot> slotList;
    public GameObject selectedSlot;
    public int slots = 6;
    public int maxSlots = 8;

    public Item itemToCombine;
    public int slotToCombine;

    public iStates state;

    public GameObject statusCanvas;

    private PlayerControls controls;
    public PlayerDamage playerDamage;
    public UIManager uiManager;
    public EventSystem eventSystem;

    private void Awake()
    {
        // Get reference to player controls
        controls = PlayerInputManager.controls;
        // Get a reference to uimanager
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();
        //Get a reference to player damage
        playerDamage = transform.parent.GetComponent<PlayerDamage>();
        // Get a reference to event system
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();

        statusCanvas = transform.Find("Canvas").gameObject;

        controls.Player.Action.performed += ctx =>
        {
            Interact();
        }; 
        controls.Player.Status.performed += ctx =>
        {
            OpenStatus();
        };
        controls.UI.Exit.performed += ctx =>
        {
            CloseStatus();
        };
        controls.UI.Cancel.performed += ctx =>
        {
            Back();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        itemList = new List<Item>();
        slotList = new List<Slot>();
        for (int i = 1; i <= maxSlots; i++)
        {
            Slot slot = GameObject.Find("Slot_" + i)?.GetComponent<Slot>();
            if (slot == null)
            {
                Debug.LogError("Slot " + i + " not found or missing Slot component.");
            }
            else
            {
                slotList.Add(slot);
                if (i > slots)
                    slotList[i - 1].gameObject.SetActive(false);
            }
        }
        selectedSlot = slotList[0]?.transform.Find("SelectButton")?.gameObject;
        statusCanvas.SetActive(false);
    }

    public void ChangeState(iStates iState)
    {
        state = iState;
        switch(iState)
        {
            case iStates.Default:
                DefaultUpdate();
                break;
            case iStates.SlotMenu:
                SlotMenuUpdate();
                break;
            case iStates.Combine:
                CombineUpdate();
                break;
        }
    }

    public void DefaultUpdate()
    {
        foreach (Slot slot in slotList)
        {
            slot.SetCombineButton(false);
            slot.SetSelectButton(true); 
            slot.SetSlotMenu(false);
            itemToCombine = null;
            slotToCombine = 0;
        }
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(selectedSlot);
    }
    public void SlotMenuUpdate()
    {
        foreach (Slot slot in slotList)
        {
            slot.SetCombineButton(false);
            slot.SetSelectButton(false);
            slot.SetSlotMenu(false);
            itemToCombine = null;
            slotToCombine = 0;
        }
        //eventSystem.SetSelectedGameObject(null);
    }
    public void CombineUpdate()
    {
        foreach (Slot slot in slotList)
        {
            slot.SetCombineButton(true);
            slot.SetSelectButton(false);
            slot.SetSlotMenu(false);
        }
        //eventSystem.SetSelectedGameObject(null);
    }

    public void AddItem(Item item)
    {
        if (itemList.Count < slots)
        {
            Debug.Log("Adding " + item.name + " to inventory");
            itemList.Add(item);
            slotList[itemList.Count - 1].item = item;
        }
        else
        {
            Debug.Log("No space in inventory");
        }
    }
    public void ReplaceItemAt(Item item, int index)
    {
        if (index < 0 || index >= itemList.Count)
        {
            return;
        }
        itemList[index] = item;
    }
    public void RemoveItem(Item item)
    {
        itemList.Remove(item);
        for (int i = 0; i < slots; i++)
        {
            if (i < itemList.Count)
                slotList[i].item = itemList[i];
            else
                slotList[i].item = null;
        }
    }
    public void RemoveItemAt(int index)
    {
        if (index < 0)
        {
            index = 0;
        }
        else if (index >= itemList.Count)
        {
            index = itemList.Count - 1;
        }
        itemList.RemoveAt(index);
        for (int i = 0; i < slots; i++)
        {
            if (i < itemList.Count)
                slotList[i].item = itemList[i];
            else
                slotList[i].item = null;
        }
    }

    public void Interact()
    {
        bool isHit = false;
        Vector3 origin = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        // Set the direction of the cast
        Vector3 direction = transform.forward;  // Casting in the forward direction of the object

        float radius = 0.25f;

        // Set the max distance the box can travel
        float maxDistance = 0.5f;  // Adjust the distance as needed

        // Optionally, set the layer mask to filter which objects to hit
        LayerMask layerMask = LayerMask.GetMask("Interactable");  // Specify the layer(s) the box should interact with

        // Perform the sphere cast away from player
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, maxDistance, layerMask);

        // Check if something was hit
        foreach (RaycastHit hit in hits)
        {
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.gameObject.GetComponent<ItemPickup>())
            {
                ItemPickup itemPickup = hit.collider.gameObject.GetComponent<ItemPickup>();
                itemPickup.Interact();
                Debug.Log("Hit: " + hit.collider.name);
                return;
            }
            else
            {
                Debug.Log("No hit");
            }
        }

        // Perfrom the sphere cast towards the player, in case the player's origin was inside the interactable
        if(!isHit)
        {
            // Update the origin to be the end point of the previous cast
            Vector3 newOrigin = origin + direction * maxDistance;
            // Reverse the direction to cast back towards the player
            Vector3 newDirection = -direction;
            // Perform the sphere cast away from player
            hits = Physics.SphereCastAll(newOrigin, radius, newDirection, maxDistance, layerMask);

            // Check if something was hit
            foreach (RaycastHit hit in hits)
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.gameObject.GetComponent<ItemPickup>())
                {
                    ItemPickup itemPickup = hit.collider.gameObject.GetComponent<ItemPickup>();
                    itemPickup.Interact();
                    Debug.Log("Hit: " + hit.collider.name);
                    return;
                }
                else
                {
                    Debug.Log("No hit");
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        // Set the color of the Gizmos
        Gizmos.color = Color.red;

        Vector3 origin = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);

        // Draw a ray from the object indicating the direction of the box cast
        Gizmos.DrawRay(origin, transform.forward * 0.5f);
        // Set the color of the Gizmos
        Gizmos.color = new Color(0, 1 ,0.5f, 0.25f);

        Gizmos.DrawSphere(origin, 0.25f);
    }
    public void OpenStatus()
    {
        if (!uiManager.uiActive)
        {
            uiManager.StartUI();
            statusCanvas.SetActive(true);
            selectedSlot = slotList[0].gameObject;
            ChangeState(iStates.Default);
        }
        
    }
    public void CloseStatus()
    {
        if (statusCanvas.activeSelf)
        {
            ChangeState(iStates.Default);
            uiManager.EndUI();
            statusCanvas.SetActive(false);
        }
    }
    public void Back()
    {
        if(state == iStates.Combine)
        {
            ChangeState(iStates.Default);
        }
        else if(state == iStates.SlotMenu)
        {
            ChangeState(iStates.Default);
        } else
        {
            CloseStatus();
        }
    }
    public void PrintList()
    {
        foreach (Item item in itemList)
        {
            Debug.Log(item.name);
        }
    }
}
