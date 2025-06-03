using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmptyCanAndSiphon : Key
{
    /// <summary>
    /// Cosntructor for a full fuel can
    /// </summary>
    /// <param name="playerInventory">Reference to player inventory script</param>
    /// <param name="uses">Number of uses before this key is automatically discarded</param>
    public EmptyCanAndSiphon(PlayerInventory playerInventory, int uses) : base(playerInventory, "FuelCanAndSiphon", uses)
    {
        name = "Empty Can and Siphon";
        description = "A fuel can with a siphon attached.";
        discardMessage = "Fuel can has been refilled.";

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }
}
