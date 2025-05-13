using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateKey : Key
{
    /// <summary>
    /// Cosntructor for a gate key
    /// </summary>
    /// <param name="playerInventory">Reference to player inventory script</param>
    /// <param name="uses">Number of uses before this key is automatically discarded</param>
    public GateKey(PlayerInventory playerInventory, int uses) : base(playerInventory, "GateKey", uses)
    {
        name = "Bolt Cutters";
        description = "A tool used for cutting bolts, chains, and padlocks.";
        plural = true;

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }
}
