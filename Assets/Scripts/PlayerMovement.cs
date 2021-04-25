﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float playerHeight = 2f;
    Rigidbody rb;

    //Houses variables pertaining to movement and orirntation
    [Header("Movement")]
    public float movementSpeed = 8f;
    float horizontalMovement;
    float verticalMovement;

    Vector3 movementDirection;
    Vector3 slopeMovementDirection;

    //[SerializeField] GameObject playerCollider;

    public float movementMultiplier = 10f;
    //[SerializeField] float airMovementMultiplier = 0.25f;
    [SerializeField] Transform orientation;

    [Header("Jump")]
    public float jumpForce = 10f;

    //Keep track off all control keys
    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode dashKey = KeyCode.LeftShift; //StickyKeys is an issue - add delay to circumvent???
    [SerializeField] KeyCode walkKey = KeyCode.LeftControl;


    //Variables for friction/drag 
    [Header("Friction/Drag")]
    float floorDrag = 6f;
    float airDrag = 1f;

    //variables for ground-detection
    [Header("Floor Detection")]
    [SerializeField] LayerMask floorMask;
    bool isOnFloor;
    //float floorDistcance = 0.5f;
    RaycastHit slopeHit;


    //all other variables
    //[Header("Misc")]
    //bool crouchingState = false;

    /*OnFloor() checks if player is on ground
     * part of ground detection system
     */
    private bool OnFloor() 
    {
        //array of raycasts? to check if touching "floor"
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 3f, Quaternion.Euler(0, 0, 0), floorMask);
        bool check = false;
        //cycle through array >> if none true then not on floor, if just one, on floor
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i]) 
            {
                check = true;
            }
        }
        return check;
    }

    /*OnSlope() checks if player is on uneven ground
     * part of ground detection system
     */
    private bool OnSlope()
    {
        //simple downward raycast check |
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight/2 +0.5f))
        {
            //if the player is not at a 90' angle with the worlds level
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
        return false;
    }

    //Set up player when instance is first loaded
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }


    //Update various conditions per frame
    private void Update()
    {
        //isOnFloor = Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f);
        //isOnFloor = Physics.CheckSphere(transform.position - new Vector3(0, 0.5f, 0), floorDistcance,floorMask); //constant check for collision with "floor"
        isOnFloor = OnFloor();

        PlayerInput();
        ControlDrag();

        /* //DEBUGGING CHECKS
        if (isOnFloor)
        { 
            print("FLOOR");
        }
        
        if (OnSlope())
        { 
            print("SLOPE");
        }*/

        //implementation of various function in movement
        if (Input.GetKeyDown(jumpKey) && isOnFloor)//can only jump if on floor
        {
            Jump();
        }
        /*if (!isOnFloor)//changes player velocity for better falling experience
        {
            rb.velocity += Vector3.up * Physics.gravity.y*Time.deltaTime;
        }*/
        if (Input.GetKeyDown(dashKey))//dash happens at key press
        {
            Dash();
        }
        if (Input.GetKey(walkKey))// && isOnFloor) //Dependent on if we want to allow crouching mid-air
        {
            Crouch();
        }
        if (Input.GetKeyUp(walkKey))// && isOnFloor) //Dependent on if we want to allow crouching mid-air
        {
            unCrouch();
        }
        

        slopeMovementDirection = Vector3.ProjectOnPlane(movementDirection, slopeHit.normal); // used in slope detection
    }

    //updates alongside physics engine
    private void FixedUpdate() 
    {
        MovePlayer();
    }

    //receive input from keys WASD
    void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal"); //[-1,1] ->[A,D]
        verticalMovement = Input.GetAxisRaw("Vertical"); //[-1,1] -> [W,S]

        movementDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement; 

    }


    /*Dash() adds a brief acceleration to player - intended as sprint at first but dash makes gameplay more interesting
     * essentially pushes the player object 
     */
    void Dash()
    {
        if (isOnFloor)
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * movementMultiplier * 10f, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * movementMultiplier * 5f, ForceMode.Acceleration);
        }
        
    }


    /*Crouch() allows the player to move slower as well as "Shrink down"
     */
    void Crouch()
    {
        //shink aspects of player
        transform.localScale = new Vector3(1, 0.5f, 1);
        playerHeight = playerHeight / 2;
        
        //reduce speed
        movementSpeed = 3f;


        //old code - kept for completions sake
        /*if (!crouchingState)
        {
            transform.localScale = new Vector3(1,0.5f,1);
            movementSpeed = 3f;
            playerHeight = playerHeight / 2;
            crouchingState = true;
        }
        else if(crouchingState)
        {
            transform.localScale =  new Vector3(1,1f,1);
            movementSpeed = 8f;
            playerHeight = playerHeight * 2;
            crouchingState = false;
        }*/
    }
    void unCrouch()
    {
        //return normal player dimensions
        transform.localScale = new Vector3(1, 1f, 1);
        playerHeight = playerHeight * 2;

        //increase speed
        movementSpeed = 8f;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    //Simply changes drag/friction depending on location in scene - can add more types of floor???
    void ControlDrag()
    {
        if (isOnFloor)
        {
            rb.drag = floorDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    //Function to apply inputs to player object in scene
    void MovePlayer()
    {
        if (isOnFloor && !OnSlope())
        {
            rb.AddForce(movementDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if(isOnFloor && OnSlope())
        {
            rb.AddForce(slopeMovementDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);

        }
        else if (!isOnFloor)
        {
            //using velocity instead of force as allows for "smoother" falling
            rb.velocity += Vector3.up * Physics.gravity.y * Time.deltaTime * 1.5f;
            //rb.AddForce(movementDirection.normalized * movementSpeed * movementMultiplier* airMovementMultiplier, ForceMode.Acceleration);
        }

    }
}
