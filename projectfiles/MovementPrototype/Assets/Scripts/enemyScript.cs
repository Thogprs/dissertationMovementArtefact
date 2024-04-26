using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyScript : MonoBehaviour
{
    // this enemyscript is the pMovement script but altered for autonomous AI use. there may be irrelevant or functionless
    // code lying around that has been missed, so if something seems to do nothing it is likely from the previous script and I missed it.

    public bool hit = false;
    public float speed;
    public GameObject directCamRef; // direct reference to the enemy "camera"
    public Transform grangle; //camera
    public float strafeForce;
    public bool grinding = false;
    public GameObject playerRef;
    public GameObject rocketLauncherRef;
    public GameObject ctrlPointRef;

    private GameObject rocketjumpDir;

    public GameObject leftGhost;
    public GameObject rightGhost;

    public GameObject leftJumpGhost;
    public GameObject rightJumpGhost;
    public GameObject frontJumpGhost;

    public GameObject grapplePoint = null;

    public bool activateGrapple = false;
    private int waitasec = 0;
    private int swaptimer = 50;

    public float groundSlow;
    public float jumpForce;
    public float jumpCD;
    public float airMultiplier;
    public float airSpeed;
    private float origspeed;
    bool jumpReady = true;
    private Vector2 combinedInput;
    public float speedCap = 100;
    public GameObject grappleRef;
    public bool crouched = false;
    public bool sliding = false;
    public Vector3 fakeVel; // for rails
    public int health = 1000;
    private bool explojumping = false;

    private bool stopFreakingOut = false; //stops the enemy rapidly spinning when preparing to rocketjump

    private NavMeshAgent theNav;

    public string enemyState = "roaming";
    // enemyState has 4 values: 

    // roaming: has no sight on player, walks to the point on the middle of the map.

    // attacking: moves towards and fires rockets at the player, moving around randomly

    // searching: after losing sight of the player, will attempt to move to where they are.
    // may set state to rocketjumping if a rocketjump could result in player vision.

    // rocketjumping: attempts to initiate a rocketjump ASAP. may occasionally do this at high health.

    //enemy inputs
    private float enemyInputHoriz = 0;
    private float enemyInputVert = 0;
    private bool enemyJumping = false;

    private bool aimpredict = true;

    //check ground
    public float pHeight;
    public LayerMask theGround;
    bool grounded;

    float horizInput;
    float vertInput;

    Vector3 moveDirection;

    Rigidbody theBody;

    public KeyCode jumpKey = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        origspeed = speedCap;
        theBody = GetComponent<Rigidbody>();
        theBody.freezeRotation = true;
        theNav = GetComponent<NavMeshAgent>();
        rocketjumpDir = leftJumpGhost;
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 1) // RESPAWN
        {
            grappleRef.GetComponent<grapple>().canGrapple = false;
            activateGrapple = false;
            grinding = false;
            health = 1000;
            GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("Respawnpoint");
            GameObject respobject = spawnpoints[Random.Range(0, spawnpoints.Length)];
            gameObject.transform.position = respobject.transform.position;
            theBody.velocity = new Vector3(0, 0, 0);
            playerRef.GetComponent<pMovement>().health = 1000; // "resets" the foe when this is killed, similar to the MGE mod in TF2
            enemyState = "roaming";
            theNav.enabled = true;
            stopFreakingOut = false;
        }

        // ENEMY AIMING

        

        RaycastHit hit;

        if (enemyState == "attacking" || enemyState == "searching" || enemyState == "roaming")
            if (Physics.Raycast(origin: grangle.position, direction: playerRef.GetComponent<pMovement>().grangle.transform.position - grangle.transform.position, out hit))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    if (Vector3.Distance(transform.position, playerRef.transform.position) < 100) // this is to stop the enemy oddly seeing the player through walls on spawn
                    {
                        enemyState = "attacking";
                    }
                }
                else
                {
                    if (enemyState != "roaming")
                    {
                        enemyState = "searching";
                    }
                    grappleRef.GetComponent<grapple>().canGrapple = false;
                    activateGrapple = false;
                }
            }


        if (enemyState == "searching")
        {
            waitasec = 0;
            rocketLauncherRef.GetComponent<rocketlauncher>().enemyFiring = false;
            if (!grinding)
            {
                if (grounded)
                {
                    theBody.velocity = new Vector3(0, 0, 0);
                    theNav.enabled = true;
                }
                theNav.destination = playerRef.transform.position;
                directCamRef.transform.LookAt(new Vector3(theNav.destination.x, transform.position.y + 0.6f, theNav.destination.z));
                directCamRef.GetComponent<enemycam>().freelook = true;

                vertInput = 0.1f;
                horizInput = 0;

            }
            stopFreakingOut = false;

        }
        else if (enemyState == "roaming")
        {
            waitasec = 0;
            rocketLauncherRef.GetComponent<rocketlauncher>().enemyFiring = false;
            if (!grinding)
            {
                if (grounded)
                {
                    theBody.velocity = new Vector3(0, 0, 0);
                    theNav.enabled = true;
                }
                theNav.destination = ctrlPointRef.transform.position;
                directCamRef.transform.LookAt(new Vector3(theNav.destination.x, transform.position.y + 0.6f, theNav.destination.z));
                directCamRef.GetComponent<enemycam>().freelook = true;
                vertInput = 0.1f;
                horizInput = 0;
            }

        }


        if (enemyState == "attacking")
        {
            theNav.enabled = false;
            directCamRef.GetComponent<enemycam>().freelook = false;
            if (health > 150 && health < 300)
            {
                if (grounded)
                {
                    if (!stopFreakingOut)
                    {
                        var jumpnum = Random.Range(0, 3);
                        if (jumpnum == 0)
                        {
                            rocketjumpDir = leftJumpGhost;
                        }
                        else if (jumpnum == 1)
                        {
                            rocketjumpDir = rightJumpGhost;
                        }
                        else
                        {
                            rocketjumpDir = frontJumpGhost;
                        }
                        stopFreakingOut = true;
                    }

                    enemyState = "rocketjumping";
                }
            }

            if (health > 999)
            {
                if (grounded)
                {
                    if (!stopFreakingOut)
                    {
                        var jumpnum = Random.Range(0, 3);
                        if (jumpnum == 0)
                        {
                            rocketjumpDir = leftJumpGhost;
                        }
                        else if (jumpnum == 1)
                        {
                            rocketjumpDir = rightJumpGhost;
                        }
                        else
                        {
                            rocketjumpDir = frontJumpGhost;
                        }
                        stopFreakingOut = true;
                    }


                    enemyState = "rocketjumping";
                }

                
            }
            if (playerRef.GetComponent<pMovement>().grounded)
            {
                if (aimpredict)
                {
                    directCamRef.transform.LookAt(playerRef.GetComponent<Rigidbody>().velocity / 2 + new Vector3(playerRef.transform.position.x, playerRef.transform.position.y - 1, playerRef.transform.position.z));
                }
                else
                {
                    directCamRef.transform.LookAt(new Vector3(playerRef.transform.position.x, playerRef.transform.position.y - 1, playerRef.transform.position.z));
                }
            }
            else
            {
                if (aimpredict)
                {
                    directCamRef.transform.LookAt(playerRef.GetComponent<Rigidbody>().velocity / 2 + new Vector3(playerRef.transform.position.x, playerRef.transform.position.y - 0.5f, playerRef.transform.position.z));
                }
                else
                {
                    directCamRef.transform.LookAt(new Vector3(playerRef.transform.position.x, playerRef.transform.position.y, playerRef.transform.position.z));
                }
            }
            if (waitasec > 30)
            {
                rocketLauncherRef.GetComponent<rocketlauncher>().enemyFiring = true;
            }
            else
            {
                waitasec = waitasec + 1;
            }

            //enemy grappling
            if (explojumping)
            {
                if (!grounded)
                {
                    if (theBody.velocity.y < -1)
                    {
                        // this chunk of code gets the closest grapplepoint for the AI within a range.
                        // if there are no viable ones it does not grapple.
                        // this code is from a scrapped feature in which you could rapidly accelerate
                        // to the start/ends of rails.

                        // it can be changed to only grapple to ones in front of it (may not work, untested)

                        var points = GameObject.FindGameObjectsWithTag("Grapplepoint");
                        float theDistance = 5000;
                        GameObject theClosest = null;
                        foreach (var grapplepos in points)
                        {
                            if (Vector3.Distance(transform.position, grapplepos.transform.position) < 50)
                            {
                                Vector3 dir = transform.position - grapplepos.transform.position;
                                //if (Mathf.Abs(Vector3.Angle(directCamRef.forward, dir)) > 90)
                                //{
                                if (Vector3.Distance(transform.position, grapplepos.transform.position) < theDistance)
                                {
                                    theClosest = grapplepos;
                                    theDistance = Vector3.Distance(transform.position, grapplepos.transform.position);
                                    grapplePoint = theClosest;
                                }
                                //}
                            }
                        }

                        if (grapplePoint != null)
                        {
                            enemyInputHoriz = 0;
                            enemyInputVert = 0;
                            grappleRef.GetComponent<grapple>().canGrapple = true;
                            activateGrapple = true;
                            explojumping = false;
                            Invoke("Getouttathegrapple", 2.0f);
                        }
                    }
                    else if (theBody.velocity.y < -2)
                    {
                        explojumping = false;
                    }
                }
                else
                {
                    explojumping = false;
                }
            }

            //attacking movement
            //essentially, it uses "ghosts" to the right and left of it to determine if it's going to run into a wall or not.
            //these ghosts check line of sight of the player and disallow the enemy to walk in that direction if it fails.
            //if both are walkable it switches it up at random intervals.
            if (grappleRef.GetComponent<grapple>().grappling == false)
            {
                bool leftGood = false;
                bool rightGood = false;
                if (Physics.Raycast(origin: leftGhost.transform.position, direction: playerRef.GetComponent<pMovement>().grangle.transform.position - leftGhost.transform.position, out hit))
                {
                    if (hit.transform.gameObject.tag == "Player")
                    {
                        if (Vector3.Distance(transform.position, playerRef.transform.position) < 100) // this is to stop the enemy oddly seeing the player through walls on spawn
                        {
                            leftGood = true;
                        }
                    }
                }

                //Debug.DrawRay(grangle.position, playerRef.GetComponent<pMovement>().grangle.transform.position - grangle.transform.position);
                //Debug.DrawRay(rightGhost.transform.position, playerRef.GetComponent<pMovement>().grangle.transform.position - rightGhost.transform.position, Color.green);
                //Debug.DrawRay(leftGhost.transform.position, playerRef.GetComponent<pMovement>().grangle.transform.position - leftGhost.transform.position, Color.red);

                if (Physics.Raycast(origin: rightGhost.transform.position, direction: playerRef.GetComponent<pMovement>().grangle.transform.position - rightGhost.transform.position, out hit))
                {
                    if (hit.transform.gameObject.tag == "Player")
                    {
                        if (Vector3.Distance(transform.position, playerRef.transform.position) < 100) // this is to stop the enemy oddly seeing the player through walls on spawn
                        {
                            rightGood = true;
                        }
                    }
                }

                if (leftGood && rightGood)
                {
                    if (swaptimer < 1)
                    {
                        swaptimer = Random.Range(30, 200);
                        var grog = Random.Range(0, 2);
                        if (grog == 1)
                        {
                            enemyInputHoriz = -1;
                        }
                        else
                        {
                            enemyInputHoriz = 1;
                        }
                        if (swaptimer < 80 && swaptimer > 75) // enemy may occasionally jump
                        {
                            enemyJumping = true;
                        }
                        else
                        {
                            enemyJumping = false;
                        }
                    }
                    else
                    {
                        swaptimer = swaptimer - 1;
                    }

                }
                else if (leftGood)
                {
                    enemyInputHoriz = -1;
                }
                else if (rightGood)
                {
                    enemyInputHoriz = 1;
                }

                // if enemy too far move towards, if too close move back
                if (Vector3.Distance(transform.position, playerRef.transform.position) > 12)
                    enemyInputVert = 1;
                else if (Vector3.Distance(transform.position, playerRef.transform.position) < 5)
                    enemyInputVert = -1;
                else
                {
                    enemyInputVert = 0.1f;
                }
            }


        }

        Debug.Log(enemyState);




        //if (Mathf.Approximately(theBody.velocity.magnitude, 0))
        if (Mathf.Abs(theBody.velocity.magnitude) < 2.5)
        {
            sliding = false;
        }
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //{
        //    crouched = true;
        //    if (theBody.velocity.magnitude > 1)
        //    {
        //        sliding = true;
        //    }
        //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.48f, gameObject.transform.position.z);
        //}
        //if (Input.GetKeyUp(KeyCode.LeftControl))
        //{
        //    crouched = false;
        //    sliding = false;
        //    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.48f, gameObject.transform.position.z);
        //}

        if (crouched)
        {
            pHeight = 0.85f;
            gameObject.transform.localScale = new Vector3(1, 0.5f, 1);
            speedCap = 50;
        }
        else
        {
            pHeight = 1.7f;
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            speedCap = origspeed;
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, pHeight * 0.5f + 0.2f, theGround);

        if (grounded)
        {
            if (sliding)
                theBody.drag = groundSlow / 10;
            else
            {
                theBody.drag = groundSlow;
            }
        }
        else
        {
            theBody.drag = 0;
        }



        if (enemyState == "rocketjumping")
        {
            //theBody.velocity = new Vector3(theBody.velocity.x, 0, theBody.velocity.z);
            directCamRef.transform.LookAt(transform.position - (rocketjumpDir.transform.position - transform.position));
            if (rocketLauncherRef.GetComponent<rocketlauncher>().fireable)
            {
                Jump();
                rocketLauncherRef.GetComponent<rocketlauncher>().enemyFiring = true;
                stopFreakingOut = false;
            }
            else
            {
                rocketLauncherRef.GetComponent<rocketlauncher>().enemyFiring = false;
            }
            explojumping = true;
            enemyState = "attacking";

        }
    }

    private void FixedUpdate()
    {
        grappleRef.GetComponent<grapple>().canGrapple = true;
        if (grappleRef.GetComponent<grapple>().grappling == false)  // stops the player moving when grappling (too much control)
        {
            if (!grinding)
            {
                EnemyInput();
                if (!sliding) // stops moving whilst sliding
                {
                    EnemyInput();
                    theBody.rotation = grangle.rotation; // sets the player to the camera's rotation
                    Vector3 vel = theBody.velocity; // gets current velocity

                    
                    // the original Quake movement is open to the public and available on github at: https://github.com/id-Software/Quake-III-Arena/blob/master/code/game/bg_pmove.c

                    // see the pMovement script for an explanation of what's happening here

                    Vector3 desiredDirection = directCamRef.transform.forward * combinedInput.y + directCamRef.transform.right * combinedInput.x;
                    desiredDirection = new Vector3(desiredDirection.x, 0.0f, desiredDirection.z).normalized;

                    var projection = Vector3.Dot(vel, desiredDirection);



                    float speedToAdd = 0;
                    float accel = 0;

                    if (grounded)
                    {
                        speedToAdd = speedCap * speedCap - projection;
                    }
                    else
                    {
                        speedToAdd = 0.075f * speedCap - projection;
                    }

                    if (speedToAdd < 0)
                    {
                        speedToAdd = 0;
                    }

                    if (grounded)
                    {
                        accel = Mathf.Clamp(speedCap * Time.fixedDeltaTime, 0, speedToAdd);
                    }
                    else
                    {
                        accel = Mathf.Clamp(speedCap * 0.2f * Time.fixedDeltaTime, 0, speedToAdd);
                    }

                    vel += desiredDirection * accel;

                    theBody.velocity = vel;


                }
            }
            else  // IF GRINDING
            {
                explojumping = false;
                grappleRef.GetComponent<grapple>().canGrapple = false;
                activateGrapple = false;
                theNav.enabled = false;
                if (Input.GetKey(jumpKey))
                {
                    grinding = false;
                }
            }

        }
    }
    private void LateUpdate()
    {
        hit = false; // ensures you can only get hit once per frame
    }
    void EnemyInput()
    {
        horizInput = enemyInputHoriz;
        vertInput = enemyInputVert;
        combinedInput = new Vector2(horizInput, vertInput);
        if (enemyJumping && jumpReady && grounded)
        {
            jumpReady = false;
            {
                Jump();

                Invoke(nameof(ResetJump), jumpCD); // continual jump
            }
        }
    }

    void Getouttathegrapple()
    {
        grappleRef.GetComponent<grapple>().canGrapple = false;
        activateGrapple = false;
        Invoke("Activatecrouch", 0.5f);
    }

    void Jump()
    {
        theBody.velocity = new Vector3(theBody.velocity.x, 0f, theBody.velocity.z);
        theBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        jumpReady = false;
    }

    void Activatecrouch()
    {
        crouched = true;
        if (theBody.velocity.magnitude > 1)
        {
            sliding = true;
        }
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.48f, gameObject.transform.position.z);
        Invoke("Unactivatecrouch", 2.0f);
    }

    void Unactivatecrouch()
    {
        crouched = false;
        sliding = false;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.48f, gameObject.transform.position.z);
    }

    void ResetJump()
    {
        jumpReady = true;
    }
}
