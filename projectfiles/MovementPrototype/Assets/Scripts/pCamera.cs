using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pCamera : MonoBehaviour
{
    //this camera script is largely based on the tutorial by Dave / GameDevelopment on youtube: https://www.youtube.com/watch?v=f473C43s8nE

    public float sensiX;
    public float sensiY;
    public Transform grangle;
    float xrot;
    float yrot;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * (sensiX/100);
        float mouseY = Input.GetAxisRaw("Mouse Y") * (sensiY/100);

        yrot += mouseX;

        xrot -= mouseY;

        xrot = Mathf.Clamp(xrot, -90f, 90f); // stops player looking too far up/down

        transform.rotation = Quaternion.Euler(xrot, yrot, 0);
        grangle.rotation = Quaternion.Euler(0, yrot, 0);
    }
}
