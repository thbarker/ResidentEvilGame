using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandgunBullets : Item
{
    protected PlayerInventory playerInventory;
    protected List<Item> itemList;

    private PlayerShoot playerShoot;
    public int count = 15;
    public int stackSize = 45;
    /// <summary>
    /// Constructor for the HandgunBullets class that initializes the number of bullets in this stack.
    /// </summary>
    /// <param name="playerInventory">PlayerInventory refernce.</param>
    /// <param name="count">Number of bullets in this stack.</param>
    public HandgunBullets(PlayerInventory playerInventory, int count) : base("HandgunBullets")
    {
        this.playerInventory = playerInventory;
        this.count = count; 
        playerShoot = GameObject.FindWithTag("Player")?.GetComponent<PlayerShoot>();

        name = "Handgun Bullets";
        description = "9mm ammunition that can be used in various handguns";
        isKeyItem = false;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }

    public override bool Use()
    {
        count = playerShoot.Reload(count);
        Debug.Log("You have reloaded your handgun");
        if (count < 1)
            return true;
        else
            return false;
    }
    public override Item Combine(Item item)
    {
        switch (item.name)
        {
            case "Handgun Bullets":
                Debug.Log("Can Combine with Handgun Bullets");
                HandgunBullets other = (HandgunBullets)item;
                if(count + other.count > stackSize)
                {
                    other.count -= stackSize - count;
                    count = stackSize;
                    return null;
                } else
                {
                    count += other.count;
                    return this;
                }
            default:
                Debug.Log("Cannot Combine with " + item.name);
                break;
        }
        return null;
    }
    public override bool CanCombine(Item item)
    {
        switch (item.name)
        {
            case "Handgun Bullets":
                Debug.Log("Can Combine with Handgun Bullets");
                HandgunBullets other = (HandgunBullets)item;
                if (count < stackSize && other.count < stackSize)
                {
                    return true;
                }
                break;
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
}
