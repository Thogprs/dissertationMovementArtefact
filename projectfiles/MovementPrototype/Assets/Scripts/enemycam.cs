using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemycam : MonoBehaviour
{
    public Transform grangle;
    public float xrot;
    public float yrot;
    public bool freelook = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!freelook)
        {
            xrot = Mathf.Clamp(xrot, -90f, 90f); // stops enemy looking too far up/down

            transform.rotation = Quaternion.Euler(xrot, yrot, 0);
            grangle.rotation = Quaternion.Euler(0, yrot, 0);
        }
    }
}
