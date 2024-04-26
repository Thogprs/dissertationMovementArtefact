using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapple : MonoBehaviour
{
    // this script is based on a tutorial by DanisTutorials on Youtube.
    // available here: https://www.youtube.com/watch?v=Xgh4v1w5DxU
    // the functionality has been altered slightly to accommodate enemy AI, note
    // the parts related to "enemyscript" and "evilGrapple"

    public LineRenderer liner;
    private Vector3 grapplePos;
    public LayerMask grappleable;
    public Transform firepoint, camera, player;
    public float maxDistance = 99f;
    private SpringJoint theJoint;
    public bool grappling = false; //grappling bool is used for other scripts
    public bool canGrapple = true;
    public bool evilGrapple = false; // if owned by an enemy
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canGrapple)
        {
            if (!evilGrapple)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    Grapple();
                    grappling = true;
                }
                else if (Input.GetMouseButtonUp(1))
                {
                    Ungrapple();
                    grappling = false;
                }
            }
            else
            {
                if (player.gameObject.GetComponent<enemyScript>().activateGrapple)
                {
                    if (player.gameObject.GetComponent<enemyScript>().grinding == false)
                    {
                        Grapple();
                        grappling = true;
                        player.gameObject.GetComponent<enemyScript>().activateGrapple = false;
                    }
                }
            }
        }
        else
        {
            Ungrapple();
            grappling = false;
        }
        

        
    }

    private void LateUpdate()
    {
        DrawGrapple();
    }

    void Grapple()
    {
        if (!evilGrapple)
        {
            RaycastHit hit;
            if (Physics.Raycast(origin: camera.position, direction: camera.forward, out hit, maxDistance, grappleable))
            {
                grapplePos = hit.point;
                theJoint = player.gameObject.AddComponent<SpringJoint>();
                theJoint.autoConfigureConnectedAnchor = false;
                theJoint.connectedAnchor = grapplePos;

                float distanceFromPos = Vector3.Distance(a: player.position, b: grapplePos);

                theJoint.maxDistance = distanceFromPos * 0.8f;
                theJoint.minDistance = distanceFromPos * 0.0f;

                theJoint.spring = 4.5f;
                theJoint.damper = 7f;
                theJoint.massScale = 4.5f;

                liner.positionCount = 2;
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(origin: camera.position, direction: player.gameObject.GetComponent<enemyScript>().grapplePoint.transform.position - camera.position, out hit, 999f, grappleable))
            {
                grapplePos = hit.point;
                theJoint = player.gameObject.AddComponent<SpringJoint>();
                theJoint.autoConfigureConnectedAnchor = false;
                theJoint.connectedAnchor = grapplePos;

                float distanceFromPos = Vector3.Distance(a: player.position, b: grapplePos);

                theJoint.maxDistance = distanceFromPos * 0.8f;
                theJoint.minDistance = distanceFromPos * 0.0f;

                theJoint.spring = 4.5f;
                theJoint.damper = 7f;
                theJoint.massScale = 4.5f;

                liner.positionCount = 2;
            }
            
        }
        
    }

    void Ungrapple()
    {
        liner.positionCount = 0;
        Destroy(theJoint);
    }

    void DrawGrapple()
    {
        if (!theJoint) return;
        liner.SetPosition(0, transform.position);
        liner.SetPosition(1, grapplePos);
    }
}

