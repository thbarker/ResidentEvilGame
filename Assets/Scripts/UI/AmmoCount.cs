using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoCount : MonoBehaviour
{
    private PlayerShoot playerShoot;
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Awake()
    {
        playerShoot = GameObject.FindWithTag("Player")?.GetComponent<PlayerShoot>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = playerShoot.magazine.ToString();
        if (playerShoot.magazine == 0)
        {
            text.color = Color.red;
        }
        else if (playerShoot.magazine == playerShoot.magazineSize)
        {
            text.color = Color.yellow;
        }
        else
        {
            text.color = Color.white;
        }
    }
}
