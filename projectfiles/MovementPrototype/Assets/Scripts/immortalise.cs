using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class immortalise : MonoBehaviour
{
    public bool activated = false;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
           
            player.GetComponent<pMovement>().health = 1000;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            activated = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            activated = false;
        }
    }
}
