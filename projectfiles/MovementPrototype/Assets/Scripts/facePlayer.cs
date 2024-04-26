using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facePlayer : MonoBehaviour
{
    public GameObject playerRef;
    
    // simple script that makes the capsule of the enemy face the player. this is to provide a stable
    // player-facing parent for the ghosts used for movement logic

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(playerRef.transform.position.x, transform.position.y, playerRef.transform.position.z));
    }
}
