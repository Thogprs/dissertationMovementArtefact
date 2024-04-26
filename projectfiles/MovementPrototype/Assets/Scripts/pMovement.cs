using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pMovement : MonoBehaviour
{
    public bool hit = false;
    public float speed;
    public Transform grangle; //the camera angle
    public float strafeForce;
    public bool grinding = false;

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
    public int health = 1000;
    public GameObject healthtext;
    bool standup = true;

    //check ground
    public float pHeight;
    public LayerMask theGround;
    public bool grounded;

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
    }

    // Update is called once per frame
    void Update()
    {
        healthtext.GetComponent<TextMesh>().text = health.ToString();

        if (Input.GetKeyDown(KeyCode.P))
        {
            health = health + 1000;
        }

        if (health < 1) // RESPAWN
        {
            grappleRef.GetComponent<grapple>().canGrapple = false;
            grinding = false;
            health = 1000;
            GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("Respawnpoint");
            GameObject respobject = spawnpoints[Random.Range(0, spawnpoints.Length)];
            gameObject.transform.position = respobject.transform.position;
            //Debug.Log(Random.Range(0, spawnpoints.Length));
            theBody.velocity = new Vector3(0, 0, 0);
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject thing in enemies)
            {
                if (thing.gameObject.GetComponent<enemyScript>())
                {
                    thing.GetComponent<enemyScript>().health = 1000; // heals all enemies upon player death (there is only one enemy normally though)
                    thing.GetComponent<enemyScript>().enemyState = "roaming";
                }
            }
        }

        if (grinding)
        {
            if (Input.GetKeyDown(jumpKey))
            {
                grinding = false;
            }
        }

        //if (Mathf.Approximately(theBody.velocity.magnitude, 0))
        if (Mathf.Abs(theBody.velocity.magnitude) < 2.5)
        {
            sliding = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (grinding == false)
            {
                standup = true;
                crouched = true;
                if (theBody.velocity.magnitude > 1)
                {
                    sliding = true;
                }
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.48f, gameObject.transform.position.z);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (standup)
            {
                crouched = false;
                sliding = false;
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.48f, gameObject.transform.position.z);
            }
        }
        
        if (crouched)
        {
            pHeight = 0.85f;
            gameObject.transform.localScale = new Vector3(1,0.5f,1);
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void FixedUpdate()
    {
        grappleRef.GetComponent<grapple>().canGrapple = true;
        if (grinding)
        {
            standup = false;
            if (crouched || sliding)
            {
                sliding = false;
                crouched = false;
            }
            grappleRef.GetComponent<grapple>().canGrapple = false;
        }
        if (grappleRef.GetComponent<grapple>().grappling == false)  // stops the player moving when grappling (too much control)
        {
            if (!grinding)
            {
                PlayerInput();
                if (!sliding) // stops moving whilst sliding
                {
                    PlayerInput();

                    theBody.rotation = grangle.rotation; // sets the player to the camera's rotation
                    Vector3 vel = theBody.velocity; // gets current velocity

                    // the fixedupdate code below is based on the original Quake movement codde, which is  open to the public
                    // and available on github at: https://github.com/id-Software/Quake-III-Arena/blob/master/code/game/bg_pmove.c

                    // essentially, this code is calculating what direction the player wishes to move gathered through camera
                    // direction and keys pressed, and continually adds a speed value to that direction.

                    // The amount of speed applied per instance is intended to* be limited if it would result in exceeding the max player movement speed.
                    // This has the benefit of not setting it to the cap and slowing you down if you are already exceeding it,
                    // which was an issue encountered before.

                    // This is a nonissue and not needed in simple platforming games, but when you have external factors like explosions
                    // propelling you past your normal max speed it can get complicated.

                    // *However, the code  mimics the famous Quake oversight which the player's desired direction can be 
                    // manipulated to allow for speed over the cap by gradually rotating into the desired direction.

                    // this bug is caused due to calculating the dot product of the velocity and the desired direction instead
                    // of the length of the player's velocity, which would solve the problem.

                    // The bug can be observed by jumping forwards, holding nothing but A and slowly turning left.
                    // You will gain speed, and if you hold down jump and maintain the movement you will continue gaining net speed.

                    // i have provided an example line on how to fix it below (the bug is intended to be enabled, however)

                    
                    
                    // this gets the wish direction by getting the player's horizontal inputs and direction.
                    Vector3 desiredDirection = transform.right * combinedInput.x + transform.forward * combinedInput.y;
                    desiredDirection = new Vector3(desiredDirection.x, 0, desiredDirection.z).normalized;

                    var projection = Vector3.Dot(vel, desiredDirection);
                    // uncomment below and comment the previous line if you want to see movement with airstrafing disabled
                    //var projection = vel.magnitude;


                    float speedToAdd = 0;
                    float accel = 0;

                    // the dot product is subtracted from the max speed.

                    if (grounded)
                    {
                        speedToAdd = speedCap * speedCap - projection;
                    }
                    else
                    {
                        speedToAdd = speedCap * 0.07f -projection; // less speed if in air
                    }

                    // if below 0, stops it interfering if the player is already over the cap

                    if (speedToAdd < 0)
                    {
                        speedToAdd = 0; 
                    }


                    // these clamp the acceleration value to not go over the cap.
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
            }
            
        }
    }
    private void LateUpdate()
    {
        hit = false; // ensures you can only get hit once per frame
    }
    void PlayerInput()
    {
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        combinedInput = new Vector2(horizInput, vertInput);
        if(Input.GetKey(jumpKey) && jumpReady && grounded)
        {
            jumpReady = false;
            {
                Jump();

                Invoke(nameof(ResetJump), jumpCD); // continual jump
            }
        }
    }

    // below is old, non Quake-like code for movement. air control was questionable at best.
    // it was based on the tutorial by Dave / GameDevelopment on youtube:https://www.youtube.com/watch?v=f473C43s8nE

    /*
    void Move()
    {
        moveDirection = grangle.forward * vertInput + grangle.right * horizInput;

        if (grounded)
        {
            theBody.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);

        }
        
        else
        {
            Vector3 newVelocity = theBody.velocity + (((moveDirection.normalized * speed * 10f) / theBody.mass) * Time.deltaTime);

            Vector3 flatVel = new Vector3(theBody.velocity.x, 0f, theBody.velocity.z);

            if (flatVel.magnitude >= newVelocity.magnitude)
            {
                theBody.AddForce(moveDirection.normalized * airSpeed * airMultiplier * 10f, ForceMode.Force);
            }
            else
            {

            }
            /*
            if (theBody.velocity >

            theBody.AddForce(moveDirection.normalized * speed * airMultiplier * 10f, ForceMode.Force);

            if (theBody.velocity.x > airSpeed)
            {
                theBody.velocity = new Vector3(beforeX, theBody.velocity.y, theBody.velocity.z);
            }
            else if (theBody.velocity.x < (airSpeed - airSpeed - airSpeed))
            {
                theBody.velocity = new Vector3(beforeX, theBody.velocity.y, theBody.velocity.z);
            }

            if (theBody.velocity.z > airSpeed)
            {
                theBody.velocity = new Vector3(theBody.velocity.x, theBody.velocity.y, beforeZ);
            }
            else if (theBody.velocity.z < (airSpeed - airSpeed - airSpeed))
            {
                theBody.velocity = new Vector3(theBody.velocity.x, theBody.velocity.y, beforeZ);
            }


        }
    }
    */
    /*
    void SpeedLimit()
    {
        Vector3 flatVel = new Vector3(theBody.velocity.x, 0f, theBody.velocity.z);

        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            if (grounded)
            {
                theBody.velocity = new Vector3(limitedVel.x, theBody.velocity.y, limitedVel.z);
            }
            
        }
    }
    */

    void Jump()
    {
        theBody.velocity = new Vector3(theBody.velocity.x, 0f, theBody.velocity.z);
        theBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        jumpReady = false;
    }

    //void Railgrab()
    //{
    //
        // this function gets the closest rail start/end that is in front of the player within a certain distance.

        // it was decided this feature will not be implemented, but the code will remain commented as it may be useful.
        
        // - it was in fact useful.

        //var railends = GameObject.FindGameObjectsWithTag("Railend");
        //float theDistance = 5000;
        //GameObject theClosest = null;
        //foreach (var railpoint in railends)
        //{
        //    if (Vector3.Distance(transform.position, railpoint.transform.position) < 20)
        //    {
        //        Vector3 dir = transform.position - railpoint.transform.position;
        //        if (Mathf.Abs(Vector3.Angle(transform.forward, dir)) > 90)
        //        {
        //            if (Vector3.Distance(transform.position, railpoint.transform.position) < theDistance)
        //            {
        //                theClosest = railpoint;
        //                theDistance = Vector3.Distance(transform.position, railpoint.transform.position);
        //                jooptorail = theClosest;
        //            }
        //        }
        //    }
        //}
    //}

    void ResetJump()
    {
        jumpReady = true;
    }


}
