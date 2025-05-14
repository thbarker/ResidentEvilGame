using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

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
    public PlayerMovement playerMovement;
    public UIManager uiManager;
    public MessageHandler messageHandler;
    public EventSystem eventSystem;
    public TextMeshProUGUI messageText;

    private void Awake()
    {
        // Get reference to player controls
        controls = PlayerInputManager.controls;
        // Get a reference to uimanager
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();
        //Get a reference to player damage
        playerDamage = transform.parent.GetComponent<PlayerDamage>();
        //Get a reference to player movement
        playerMovement = transform.parent.GetComponent<PlayerMovement>();
        // Get a reference to the message handler
        messageHandler = GameObject.FindWithTag("Player")?.transform.Find("MessageHandler")?.GetComponent<MessageHandler>();
        // Get a reference to event system
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();

        statusCanvas = transform.Find("Canvas").gameObject;

        controls.Player.Action.canceled += ctx =>
        {
            Interact();
        }; 
        controls.Player.Status.performed += ctx =>
        {
            if (playerMovement.StateMachine.CurrentPlayerState == playerMovement.MoveState)
            {
                OpenStatus();
            }
        };
        controls.UI.Submit.performed += ctx =>
        {
            SubmitCanceled();
        };
        controls.UI.Submit.canceled += ctx =>
        {
            SubmitCanceled();
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

        AddItem(new HandgunBullets(this, 15));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            AddItem(new HandgunBullets(this, 15));
        }
        if (Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            AddItem(new GateKey(this, 1));
        }
        if(Input.GetKeyDown(KeyCode.M)) 
        {
            Debug.Log("Attempting to send messages");
            messageHandler.QueueMessage("This is the first message!");
            messageHandler.QueueMessage("This is the second message!");
            messageHandler.QueueMessage("This is the third message!");
        }
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
            if (slot.item != null) slot.UpdateIcon(1, true);
            else slot.UpdateIcon(0, false);
            slot.SetSlotMenu(false);
            itemToCombine = null;
            slotToCombine = 0;
        }
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
    }
    public void CombineUpdate()
    {
        foreach (Slot slot in slotList)
        {
            slot.SetCombineButton(false);
            slot.SetSelectButton(false);
            slot.SetSlotMenu(false);
        }
    }

    public void AddItem(Item item)
    {
        foreach (Item listItem in  itemList)
        {
            // If the item is already in the inventory and there is room to stack, do so
            if(listItem.name == item.name && item is HandgunBullets)
            {
                HandgunBullets bullets = (HandgunBullets)item;
                HandgunBullets listBullets = (HandgunBullets)listItem;
                if (bullets.count + listBullets.count <= bullets.stackSize)
                {
                    listBullets.count += bullets.count;
                    return;
                }
            }
        }
        if (itemList.Count < slots)
        {
            Debug.Log("Adding " + item.name + " to inventory");
            itemList.Add(item);
            slotList[itemList.Count - 1].item = item;
            UpdateIcons();
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
        UpdateIcons();
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
        UpdateIcons();
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
        UpdateIcons();
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
            if (hit.collider.gameObject.GetComponent<Interactable>())
            {
                Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                interactable.Interact();
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
                    Interactable interactable = hit.collider.gameObject.GetComponent<Interactable>();
                    interactable.Interact();
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
            SetMessageText("");
            ChangeState(iStates.Default);
            if (slotList[0].item != null)
            {
                selectedSlot = slotList[0].selectButton;
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(selectedSlot);
            }
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
        if (messageHandler.IsActive())
        {
            NextMessage();
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
    public void StartMessageSequence()
    {
        if (statusCanvas.activeSelf)
        {
            return;
        }
        uiManager.StartUI();
        messageHandler.ShowMessage();
    }
    public void NextMessage()
    {
        if(messageHandler.IsEmpty())
        {
            messageHandler.HideMessage();
            uiManager.EndUI();
        } else
        {
            messageHandler.ShowMessage();
        }
    }
    public void SubmitCanceled()
    {
    }
    public void UpdateIcons()
    {
        foreach (Slot slot in slotList)
        {
            if (slot.item != null)
                slot.UpdateIcon(1, true);
            else
                slot.UpdateIcon(0, false);
        }
    }
    /// <summary>
    /// This function checks the player's inventory for a key with name keyName.
    /// </summary>
    /// <param name="keyName">Name of the key that should be checked for.</param>
    /// <returns>The first item with the same name as the keyName, or null if nothing was found.</returns>
    public Key CheckForKey(string keyName)
    {
        foreach (Item listItem in itemList)
        {
            if (listItem.name == keyName)
            {
                return (Key)listItem;
            }
        }
        return null;
    }
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
    public void PrintList()
    {
        foreach (Item item in itemList)
        {
            Debug.Log(item.name);
        }
    }
}
