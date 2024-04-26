using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rocketlauncher : MonoBehaviour
{
    public Transform rocketSpawnPoint;
    public GameObject rocketPrefab;
    public float rocketForce = 40;
    public bool fireable = true;
    public bool enemyFiring = false;
    private int ammocount = 4;
    private int firetimer = 0;
    private int reloadtimer = 0;
    public GameObject ammotext = null;
    public bool evilRocketlauncher = false; // if owned by an enemy

    // Update is called once per frame
    private void Update()
    {
        if (!evilRocketlauncher)
        {
            ammotext.GetComponent<TextMesh>().text = ammocount.ToString();
        }
    }

    void FixedUpdate()
    {
        if (firetimer > 0)
        {
            firetimer -= 1;
        }

        if (reloadtimer > 0)
        {
            reloadtimer -= 1;
        }
        else
        {
            if (ammocount < 4)
            {
                ammocount += 1;
                reloadtimer = 50;
            }
        }
        if (ammocount > 0 && firetimer == 0)
        {
            fireable = true;
        }

        if (evilRocketlauncher)
        {
            if (enemyFiring)
            {
                if (fireable)
                {
                    var rocket = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);
                    rocket.GetComponent<Rigidbody>().velocity = rocketSpawnPoint.forward * rocketForce;
                    rocket.GetComponent<Rocket>().enemyRocket = true;
                    fireable = false;
                    firetimer = 25;
                    reloadtimer = 75;
                    ammocount -= 1;
                }

            }
        }
        else
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (fireable)
                {
                    var rocket = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);
                    rocket.GetComponent<Rigidbody>().velocity = rocketSpawnPoint.forward * rocketForce;
                    if (!evilRocketlauncher)
                    rocket.GetComponent<Rocket>().enemyRocket = false;
                    fireable = false;
                    firetimer = 25;
                    reloadtimer = 75;
                    ammocount -= 1;
                }
            }
        }
    }
}
