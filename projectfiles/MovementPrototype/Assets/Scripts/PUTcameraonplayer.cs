using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUTcameraonplayer : MonoBehaviour
{
    public Transform campos;
    // Start is called before the first frame update
    void Update()
    {
        transform.position = campos.position;
    }
}
