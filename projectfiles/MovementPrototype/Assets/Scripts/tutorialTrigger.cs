using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tutorialTrigger : MonoBehaviour
{
    public GameObject textRef;

    // Start is called before the first frame update
    void Start()
    {
        textRef.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            textRef.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            textRef.SetActive(false);
        }
    }
}
