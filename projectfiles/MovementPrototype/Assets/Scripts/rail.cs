using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rail : MonoBehaviour
{
    public GameObject startRef;
    public GameObject endRef;
    public GameObject startLaunch;
    public GameObject endLaunch;
    private float distance = 0;
    private bool active = false;
    private bool forwards = true;
    private GameObject playerRef = null;
    public float railspeed = 0.02f;
    public bool evilRail = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (playerRef != null)
        {
            if ((evilRail ? playerRef.GetComponent<enemyScript>().grinding : playerRef.GetComponent<pMovement>().grinding) == false) //should probably use ternary operators more often, useful
            {
                active = false;
                if (forwards)
                {
                    var dir = endLaunch.transform.position - transform.position;
                    dir = dir.normalized;
                    playerRef.GetComponent<Rigidbody>().AddForce(dir * (railspeed * 30000));
                }
                else
                {
                    var dir = startLaunch.transform.position - transform.position;
                    dir = dir.normalized;
                    playerRef.GetComponent<Rigidbody>().AddForce(dir * (railspeed * 30000));
                }
                playerRef = null;
            }
            else
            {
                if (active)
                {
                    transform.position = Vector3.Lerp(startRef.transform.position, endRef.transform.position, distance);
                    playerRef.transform.position = new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z);
                    if (forwards)
                    {
                        distance += (railspeed / gameObject.transform.parent.transform.localScale.x) * 2.2f; // this is multiplied by the parent's x transform to account for longer rails. *2.2f brings it up to roughly ideal speeds.
                        if (distance >= 1)
                        {
                            active = false;
                            if (evilRail)
                            {
                                playerRef.GetComponent<enemyScript>().grinding = false;
                            }
                            else
                            {
                                playerRef.GetComponent<pMovement>().grinding = false;
                            }
                            
                        }
                        else
                        {
                            playerRef.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        }
                    }
                    else
                    {
                        distance -= (railspeed / gameObject.transform.parent.transform.localScale.x) * 2.2f;
                        playerRef.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        if (distance <= 0)
                        {
                            active = false;
                            if (evilRail)
                            {
                                playerRef.GetComponent<enemyScript>().grinding = false;
                            }
                            else
                            {
                                playerRef.GetComponent<pMovement>().grinding = false;
                            }
                            
                        }
                        else
                        {
                            playerRef.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                        }
                    }

                    
                }
            }
        }
        
    }


    public void Attachto(GameObject entity)
    {
        // this part basically scans along the lerp between the start and end points for the closest (within reason) position to the player upon contact.
        float lastDist = Vector3.Distance(Vector3.Lerp(startRef.transform.position, endRef.transform.position, 0.5f), entity.transform.position);
        bool increase = true;
        bool first = true;
        float increment = 0.5f;
        for (int i = 0; i < 120; i++)
        {
            if (increase)
            {
                increment = increment + 0.01f;
            }
            else
            {
                increment = increment - 0.01f;
            }
            if (Vector3.Distance(Vector3.Lerp(startRef.transform.position, endRef.transform.position, increment), entity.transform.position) < lastDist) // IF NEW FLOAT IS SMALLER
            {
                lastDist = Vector3.Distance(Vector3.Lerp(startRef.transform.position, endRef.transform.position, increment), entity.transform.position);
            }
            else // else if bigger (further away)
            {
                if (first)
                {
                    first = false;
                    increase = false;
                    lastDist = Vector3.Distance(Vector3.Lerp(startRef.transform.position, endRef.transform.position, increment), entity.transform.position);
                }
                else
                {
                    break;
                }
            }
        }
        // it then sucks them to itself and starts lerping. it uses the player's total speed at the time of impact as reference for how fast to go.
        gameObject.transform.position = Vector3.Lerp(startRef.transform.position, endRef.transform.position, increment);
        if (entity.tag == "Player" || entity.tag == "Enemy")
        {
            distance = increment;
            if (entity.tag == "Player")
            {
                entity.GetComponent<pMovement>().grinding = true;
                entity.GetComponent<pMovement>().crouched = false;
                entity.GetComponent<pMovement>().sliding = false;
            }

            if (entity.tag == "Enemy")
            {
                entity.GetComponent<enemyScript>().grinding = true;
                entity.GetComponent<enemyScript>().crouched = false;
                entity.GetComponent<enemyScript>().sliding = false;
            }

            playerRef = entity;
            
            active = true;
            if (entity.tag == "Player")
            {
                railspeed = playerRef.GetComponent<Rigidbody>().velocity.magnitude / 600;
            }
            else if (entity.tag == "Enemy")
            {
                float speedval = playerRef.GetComponent<Rigidbody>().velocity.magnitude;
                if (speedval < 10)
                {
                    railspeed = 0.01f; // the enemy gets a minimum railspeed in the event it touches a rail with the nav agent active (rigidbody has no actual velocity)
                }
                else
                {
                    railspeed =  speedval / 600;
                }
            }

            // this code determines what direction to send the player. it uses the dot product of the player's velocity and the direction of the rail
            // to change a "forwards" boolean. the usage of the dot product was inspired by the movement calculations in the pMovement script.
            Vector3 railDir = endRef.transform.position - startRef.transform.position;
            railDir = railDir.normalized;
            var backorforwards = Vector3.Dot(playerRef.GetComponent<Rigidbody>().velocity, railDir);
            if (entity.tag == "Enemy")
            {
                backorforwards = Vector3.Dot(playerRef.transform.forward, railDir);
            }
            if (backorforwards < 0)
            {
                forwards = false;
            }
            else
            {
                forwards = true;
            }

        }
    }
}
