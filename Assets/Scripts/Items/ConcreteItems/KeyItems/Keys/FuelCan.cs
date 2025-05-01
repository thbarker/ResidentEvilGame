using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelCan : Key
{
    /// <summary>
    /// Cosntructor for a full fuel can
    /// </summary>
    /// <param name="playerInventory">Reference to player inventory script</param>
    /// <param name="uses">Number of uses before this key is automatically discarded</param>
    public FuelCan(PlayerInventory playerInventory, int uses) : base(playerInventory, "FuelCan", uses)
    {

        name = "Fuel Can";
        description = "A fuel can filled with gasoline.";

        if (icon == null)
        {
            Debug.LogWarning("The icon didnt set");
        }
    }
}
