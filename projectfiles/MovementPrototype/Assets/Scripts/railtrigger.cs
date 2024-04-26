using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class railtrigger : MonoBehaviour
{
    public GameObject railRef;
    public GameObject evilRailRef;

    //this script puts either a player or an enemy onto their respective rail "grabbers" that propel them along the rail.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {
            railRef.GetComponent<rail>().Attachto(other.gameObject);
        }

        if (other.tag == "Enemy")
        {
            evilRailRef.GetComponent<rail>().Attachto(other.gameObject);
        }
    }
}
