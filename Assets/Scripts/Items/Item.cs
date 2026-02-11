using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    protected PlayerInventory playerInventory;
    public string name;
    public string description;
    public string discardMessage = "";
    public bool isKeyItem;
    public AudioClip audioClip;

    public Sprite icon;

    /// <summary>
    /// Constructor for the Item class that initializes the item's icon based on the provided iconName.
    /// </summary>
    /// <param name="iconName">Name of the icon to load from resources.</param>
    public Item(string iconName)
    {
        icon = InitializeIcon(iconName);
    }
    /// <summary>
    /// Constructor for the Item class that initializes the item's icon based on the provided iconName
    /// and the Audio Clip meant to be activated when the item is used.
    /// </summary>
    /// <param name="iconName">Name of the icon to load from resources.</param>
    /// <param name="clipName">Name of the audio clip to load from resources.</param>
    public Item(string iconName, string clipName)
    {
        icon = InitializeIcon(iconName);
        audioClip = InitializeAudioClip(clipName);
    }
    /// <summary>
    /// Virtual method for using the item. This should be overridden in derived classes for specific item behaviors.
    /// </summary>
    /// <returns>Boolean indicating if the item was successfully used and should be discarded.</returns>
    public virtual bool Use() { if (audioClip != null) { playerInventory.PlayClip(audioClip); } return false; }
    /// <summary>
    /// Virtual method to combine this item with another item. Should be overridden to specify how items combine.
    /// </summary>
    /// <param name="item">The other item to combine with.</param>
    /// <returns>The resulting item from the combination, or null if not combinable or shouldn't delete combining items.</returns>
    public virtual Item Combine(Item item) { return null; }
    /// <summary>
    /// Virtual method to combine this item with another item. Should be overridden to specify how items combine.
    /// </summary>
    /// <param name="item">The other item to combine with.</param>
    /// <returns>True if the items can combine.</returns>
    public virtual bool CanCombine(Item item) { return false; }
    /// <summary>
    /// <summary>
    /// Virtual method to examine the item. Can be overridden to provide more detailed examination behavior.
    /// </summary>
    public virtual void Examine() 
    {
        if (playerInventory != null)
        {
            playerInventory.SetMessageText(description);
        }
    }
    /// <summary>
    /// Helper method to initialize the sprite icon for the item from the specified item name.
    /// </summary>
    /// <param name="itemName">The item name used to load the corresponding icon sprite.</param>
    /// <returns>The sprite loaded for the icon, or null if not found.</returns>
    public Sprite InitializeIcon(string itemName)
    {
        Sprite icon = Resources.Load<Sprite>("Sprites/Icons/" + itemName);
        if (icon == null)
        {
            Debug.LogWarning($"Icon for {itemName} not found!");
        }
        return icon;
    }/// <summary>
     /// Helper method to initialize the audio clip for the item from the specified clip name.
     /// </summary>
     /// <param name="clipName">The audio clip name used to load the corresponding use audio.</param>
     /// <returns>The audio clip loaded for the item, or null if not found.</returns>
    public AudioClip InitializeAudioClip(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + clipName);
        if (clip == null)
        {
            Debug.LogWarning($"Audio Clip for {clipName} not found!");
        }
        return clip;
    }
}