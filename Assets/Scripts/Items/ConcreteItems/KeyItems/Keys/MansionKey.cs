using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansionKey : Key
{
    /// <summary>
    /// Cosntructor for a mansion key
    /// </summary>
    /// <param name="playerInventory">Reference to player inventory script</param>
    /// <param name="uses">Number of uses before this key is automatically discarded</param>
    public MansionKey(PlayerInventory playerInventory, int uses) : base(playerInventory, "MansionKey", uses)
    {

        name = "Mansion Key";
        description = "A key to the front door of the mansion.";

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }
}
