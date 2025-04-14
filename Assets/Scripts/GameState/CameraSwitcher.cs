using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public Transform player;
    public CinemachineVirtualCamera activeCamera;
    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        activeCamera = transform.Find("Virtual Camera")?.gameObject.GetComponent<CinemachineVirtualCamera>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            activeCamera.Priority = 1;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCamera.Priority = 0;
        }
    }
}
