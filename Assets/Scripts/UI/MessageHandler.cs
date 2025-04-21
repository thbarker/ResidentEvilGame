using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MessageHandler : MonoBehaviour
{
    public UIManager uiManager;
    public GameObject panel;
    public TextMeshProUGUI message;
    public PlayerInventory playerInventory;
    private Queue<string> messageQueue;

    private void Awake()
    {
        playerInventory = GameObject.FindWithTag("Player")?.transform.Find("Inventory")?.GetComponent<PlayerInventory>();
        uiManager = GameObject.FindWithTag("Player")?.transform.Find("UIManager")?.GetComponent<UIManager>();
        panel = transform.Find("Panel")?.gameObject;
        message = panel.transform.Find("Text")?.GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        panel.SetActive(false);
        messageQueue = new Queue<string>();
    }
    private void Update()
    {
        if (!panel.activeSelf && !IsEmpty())
            playerInventory.StartMessageSequence();
    }
    public void QueueMessage(string text)
    {
        messageQueue.Enqueue(text);
    }

    public void ShowMessage()
    {
        panel.SetActive(true);
        message.text = messageQueue.Dequeue();
    }
    public void HideMessage()
    {
        message.text = "";
        panel.SetActive(false);
    }
    public bool IsEmpty()
    {
        return messageQueue.Count == 0;
    }
    public bool IsActive()
    {
        return panel.activeSelf;
    }
}