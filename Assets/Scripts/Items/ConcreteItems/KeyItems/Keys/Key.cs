using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Key : Item
{
    protected PlayerInventory playerInventory;
    public int uses;

    public Key(PlayerInventory playerInventory, string sprite, int uses) : base(sprite)
    {
        this.playerInventory = playerInventory;

        isKeyItem = true;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }

        this.uses = uses;
    }

    public override bool Use()
    {
        Unlock();
        return false;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            default:
                break;
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return false;
    }
    public override void Examine()
    {
        Debug.Log(description);
    }

    private void Unlock()
    {
        Transform playerTransform = playerInventory.gameObject.transform;
        Vector3 origin = new Vector3(playerTransform.position.x, playerTransform.position.y + 1, playerTransform.position.z);

        // Set the direction of the cast
        Vector3 direction = playerTransform.forward;  // Casting in the forward direction of the object

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
            if (hit.collider.gameObject.GetComponent<Lockable>())
            {
                Lockable lockable = hit.collider.gameObject.GetComponent<Lockable>();
                lockable.Interact();
                Debug.Log("Hit: " + hit.collider.name);
            }
            else
            {
                Debug.Log("No hit");
            }
        }
    }
}
