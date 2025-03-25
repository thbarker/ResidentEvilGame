using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuickHealthIndicator: MonoBehaviour
{
    PlayerDamage playerDamage;
    public Image image;
    private void Awake()
    {
        playerDamage = transform.parent.GetComponent<PlayerDamage>();
    }

    void Update()
    {
        image.color = GetColorBasedOnValue(playerDamage.GetHealth());
    }

    Color GetColorBasedOnValue(float value)
    {
        // Normalize the value to 0-1 for Lerp
        float normalizedValue = Mathf.Clamp(value, 0, playerDamage.maxHealth) / playerDamage.maxHealth;

        // Define the colors at specific points
        Color red = Color.red;
        Color yellow = Color.yellow;
        Color green = Color.green;

        // Lerp between red, yellow, and green
        if (normalizedValue < 0.5f) // From 0 to 50
        {
            // Lerp from red to yellow
            return Color.Lerp(red, yellow, normalizedValue * 2); // Multiplied by 2 to normalize 0-0.5 to 0-1
        }
        else // From 50 to 100
        {
            // Lerp from yellow to green
            return Color.Lerp(yellow, green, (normalizedValue - 0.5f) * 2); // Normalized from 0.5-1 to 0-1
        }
    }
}
