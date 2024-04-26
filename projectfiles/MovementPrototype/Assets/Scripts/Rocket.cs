using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float blowRadius = 5f;
    public float blowForce = 700f;
    public bool enemyRocket = false;
    public bool doOnce = true;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!enemyRocket)
        {
            if (other.tag != "Player" && other.tag != "Rocketnocollide" && other.tag != "Railend")
            {
                Explode();
            }
        }
        else
        {
            if (other.tag != "Enemy" && other.tag != "Rocketnocollide" && other.tag != "Railend")
            {
                Explode();
            }
        }
        
    }

    void Explode()
    {
        //make explosion
        Collider[] colliders = Physics.OverlapSphere(transform.position, blowRadius);
        foreach (Collider nearbyThing in colliders)
        {
            if (nearbyThing.tag == "Player")
            {
                Rigidbody rb = nearbyThing.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (nearbyThing.GetComponent<pMovement>().hit == false)
                    {
                        float distance = Vector3.Distance(transform.position, rb.transform.position);
                        rb.AddExplosionForce(blowForce, transform.position, blowRadius);
                        if (enemyRocket) // this reduces damage if hit by own rockets
                        {
                            nearbyThing.GetComponent<pMovement>().health -= (int)((blowForce / 2 - distance * 50)*0.8f) ; // this combination of distance and the blowforce results in ~3 rockets killing someone.
                        }
                        else
                        {
                            nearbyThing.GetComponent<pMovement>().health -= (int)((blowForce / 2 - distance * 50)*0.8f) / 3;
                        }
                        nearbyThing.GetComponent<pMovement>().hit = true; // this is to stop a rare bug where you can hit yourself twice with a rocket
                    }
                }
            }

            else if (nearbyThing.tag == "Enemy")
            {
                Rigidbody rb = nearbyThing.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    if (nearbyThing.GetComponent<enemyScript>().hit == false)
                    {
                        float distance = Vector3.Distance(transform.position, rb.transform.position);
                        rb.AddExplosionForce(blowForce, transform.position, blowRadius);
                        if (!enemyRocket) // this reduces damage if hit by own rockets
                        {
                            nearbyThing.GetComponent<enemyScript>().health -= (int)((blowForce / 2 - distance * 50)*0.8f);
                        }
                        else
                        {
                            nearbyThing.GetComponent<enemyScript>().health -= (int)((blowForce / 2 - distance * 50)*0.8f) / 3;
                        }
                        nearbyThing.GetComponent<enemyScript>().hit = true; // this is to stop a rare bug where you can hit yourself twice with a rocket
                    }
                }
            }
        }


        Destroy(gameObject);
    }

}
