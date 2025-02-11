﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{
    //Variable Definitions
    [SerializeField] private float sensX = 100f; //serializefield allows unity to use private variables
    [SerializeField] private float sensY = 100f;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    //Initialisation of input variables
    float mouseX;
    float mouseY;
    float controllerX;
    float controllerY;
    public float sensitivityMultiplier = 0.02f;
    float xRotation;
    float yRotation;

    //Rotate player model
    [SerializeField] GameObject playerModel; // used to rotate player model; MUST ASSIGN IN UNINTY!!!

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // used to keep cursor in game
        Cursor.visible = false; //used to hide cursor
    }

    public void FixedUpdate()
    {
        PlayerInput();
        //adjusts the camera object to the input
        cam.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        playerModel.transform.rotation = Quaternion.Euler(0, yRotation, 0);


    }

    void PlayerInput()
    {
        //Takes in mouse movement
        //controllerX
        //controllerY
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        //takes mouse input and adds it to the player object
        yRotation += mouseX * sensX * sensitivityMultiplier;
        xRotation -= mouseY * sensY * sensitivityMultiplier;

        //restricts camera movement to a 180'
        xRotation = Mathf.Clamp(xRotation, -90f, 60f);
    }

    
}
