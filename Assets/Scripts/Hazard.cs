using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    //config params
    [SerializeField] string hazardColor;
    [SerializeField] bool isEnemy;
    [SerializeField] bool isTrap;

    [SerializeField] float moveTimerBase;
    [SerializeField] float moveSpeed = 10;

    [SerializeField] bool goingDown;
    [SerializeField] bool goingUp;
    [SerializeField] bool goingLeft;
    [SerializeField] bool goingRight;

    //ref params
    Rigidbody2D myRigidbody;
    float moveTimer;
    

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        if (goingDown) { transform.eulerAngles = new Vector3(0, 0, 0f);}
        if (goingUp) { transform.eulerAngles = new Vector3(0, 0, 180f); }
        if (goingLeft) { transform.eulerAngles= new Vector3(0,0,-90f); }
        if (goingRight) { transform.eulerAngles = new Vector3(0, 0, 90f); }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (moveTimer > 0)
        {
            myRigidbody.velocity = GetDirection() * moveSpeed;
            moveTimer -= Time.deltaTime * moveSpeed;
        }
        else if (moveTimer <= 0)
        {
            myRigidbody.velocity = Vector2.zero;
            SnapToGrid();
        }
    }

    public void ActivateHazard(string pawnColor)
    {
        if (pawnColor == hazardColor)
        {
            if (isEnemy)
            {
                ActivateEnemy();
            }
            if (isTrap)
            {
                ActivateTrap();
            }
        }
    }

    private void ActivateTrap()
    {
        gameObject.SetActive(ToggleBool());
    }

    private bool ToggleBool()
    {
        if (gameObject.activeSelf == true)
        {
            return false;
        }
        else 
        {
            return true;
        }

    }

    private void ActivateEnemy()
    {
        moveTimer = moveTimerBase;
        
    }



    private Vector2 GetDirection()
    {
        if (goingDown) { return Vector2.down; }
        else if (goingUp) { return Vector2.up; }
        else if (goingLeft) { return Vector2.left; }
        else { return Vector2.right; }
    }

    public void ReverseDirection()
    {
        ChangeDirection();
        SnapToGrid();
    }

    private void ChangeDirection()
    {
        if (goingDown)
        { 
            goingDown = false;
            goingUp = true;
            transform.eulerAngles = new Vector3(0, 0, 180f);
        }
        else if (goingUp)
        {
            goingUp = false;
            goingDown = true;
            transform.eulerAngles = new Vector3(0, 0, 0f);
        }
        else if (goingRight)
        {
            goingRight = false;
            goingLeft = true;
            transform.eulerAngles = new Vector3(0, 0, -90f);
        }
        else if (goingLeft)
        {
            goingLeft = false;
            goingRight = true;
            transform.eulerAngles = new Vector3(0, 0, 90f);
        }
    }

    private void SnapToGrid()
    {
        float newX = Mathf.RoundToInt(transform.position.x);
        float newY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector2(newX, newY);
    }
}
