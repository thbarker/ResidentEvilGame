using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FocusCamera : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public CinemachineVirtualCamera activeCamera;
    private MessageHandler messageHandler;
    private void Awake()
    {
        activeCamera = transform.Find("Virtual Camera")?.gameObject.GetComponent<CinemachineVirtualCamera>();
        messageHandler = GameObject.FindWithTag("Player")?.transform.Find("MessageHandler")?.GetComponent<MessageHandler>();
        playerMovement = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
    }
    private void Start()
    {
        messageHandler.focusCameraList.Add(this);
    }
    public void Activate()
    {
        playerMovement.HideMesh();
        activeCamera.Priority = 2;
    }
    public void Deactivate()
    {
        playerMovement.ShowMesh();
        activeCamera.Priority = 0;
    }
}
