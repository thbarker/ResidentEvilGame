using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmPickup : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject confirmationPanel;
    public TextMeshProUGUI confirmationMessage;

    private void Awake()
    {
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();
        confirmationPanel = transform.Find("ConfirmPickupPanel").gameObject;
        confirmationMessage = confirmationPanel.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        confirmationPanel.SetActive(false);
    }
    public void ShowPickupConfirmation(string itemName)
    {
        confirmationPanel.SetActive(true);
        uiManager.StartUI();
        confirmationMessage.text = "Pick up the " + itemName + "?";
    }
    public void HidePickupConfirmation()
    {
        uiManager.EndUI();
        confirmationPanel.SetActive(false);
    }
}
