using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Progress;

public class ConfirmPickup : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationMessage;
    public EventSystem eventSystem;
    public GameObject denyButton;
    public PlayerInventory playerInventory;
    public ItemPickup itemPickup;

    private void Awake()
    {
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>(); 
        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
        confirmationPanel = transform.Find("ConfirmPickupPanel").gameObject;
        confirmationMessage = confirmationPanel.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
        denyButton = confirmationPanel.transform.Find("No").gameObject;
    }
    private void Start()
    {
        confirmationPanel.SetActive(false);
    }
    public void ShowPickupConfirmation(string itemName)
    {
        uiManager.StartUI();
        confirmationPanel.SetActive(true);
        confirmationMessage.text = "Pick up the " + itemName + "?";
        eventSystem.SetSelectedGameObject(denyButton);
        Debug.Log("We're here!!!");
    }
    public void HidePickupConfirmation()
    {
        uiManager.EndUI();
        confirmationPanel.SetActive(false);
        Debug.Log("We're no longer here!!!");
    }
    public void Confirm()
    {
        HidePickupConfirmation();
        playerInventory.AddItem(itemPickup.item);
        Destroy(itemPickup.gameObject);
        itemPickup = null;
    }
    public void Deny()
    {
        itemPickup = null;
        HidePickupConfirmation();
    }
}
