using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(player)
        {
            transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        } else
        {
            Debug.Log("Player not found.");
        }
    }
}
