using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{

    //config params
    [SerializeField] PublicVars.Color myColor;
    [SerializeField] bool isEnemy;
    [SerializeField] bool isTrap;
    [SerializeField] float moveTimerBase;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] PublicVars.StartingDirection myDirection;

    //ref params
    Rigidbody2D myRigidbody;
    float moveTimer;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        switch (myDirection)
            {
            case PublicVars.StartingDirection.down: 
                transform.eulerAngles = new Vector3(0, 0, 0f);
                break;
            case PublicVars.StartingDirection.up:
                transform.eulerAngles = new Vector3(0, 0, 180f);
                break;
            case PublicVars.StartingDirection.left:
                transform.eulerAngles = new Vector3(0, 0, -90f); 
                break;
            case PublicVars.StartingDirection.right:  
                transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
            default:
                Debug.Log("No direction stored for " + name);
                break;
            }
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

    public void ActivateHazard(PublicVars.Color slimeColor)
    {
        if (slimeColor == myColor)
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
        switch (myDirection)
        {
            case PublicVars.StartingDirection.down:
                return Vector2.down;
            case PublicVars.StartingDirection.up:
                return Vector2.up;
            case PublicVars.StartingDirection.left:
                return Vector2.left;
            case PublicVars.StartingDirection.right:
                return Vector2.right;
            default:
                Debug.Log(name + " has no direction!");
                return Vector2.zero;
        }
    }

    public void ReverseDirection()
    {
        ChangeDirection();
        SnapToGrid();
    }

    private void ChangeDirection()
    {
        switch (myDirection)
        {
            case PublicVars.StartingDirection.down:
                myDirection = PublicVars.StartingDirection.up;
                transform.eulerAngles = new Vector3(0, 0, 180f);
                break;
            case PublicVars.StartingDirection.up:
                myDirection = PublicVars.StartingDirection.down;
                transform.eulerAngles = new Vector3(0, 0, 0f);
                break;
            case PublicVars.StartingDirection.right:
                myDirection = PublicVars.StartingDirection.left;
                transform.eulerAngles = new Vector3(0, 0, -90f);
                break;
            case PublicVars.StartingDirection.left:
                myDirection = PublicVars.StartingDirection.right;
                transform.eulerAngles = new Vector3(0, 0, 90f);
                break;
            default:
                Debug.Log("No direction set.");
                break;
        }
    }

    private void SnapToGrid()
    {
        float newX = Mathf.RoundToInt(transform.position.x);
        float newY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector2(newX, newY);
    }
}
