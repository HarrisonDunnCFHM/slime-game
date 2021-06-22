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
    [SerializeField] PublicVars.Direction myDirection;

    //ref params
    Rigidbody2D myRigidbody;
    LevelManager levelManager;
    float moveTimer;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        levelManager = FindObjectOfType<LevelManager>();
        switch (myDirection)
            {
            case PublicVars.Direction.down: 
                transform.eulerAngles = new Vector3(0, 0, 0f);
                break;
            case PublicVars.Direction.up:
                transform.eulerAngles = new Vector3(0, 0, 180f);
                break;
            case PublicVars.Direction.left:
                transform.eulerAngles = new Vector3(0, 0, -90f); 
                break;
            case PublicVars.Direction.right:  
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
        LayerGrid();
    }
    private void LayerGrid()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y - 10);
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
            if (levelManager.CheckLevelPlayable())
            {
                SnapToGrid();
            }
        }
    }

    public void StopHazardMove()
    {
        moveTimer = 0;
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
            case PublicVars.Direction.down:
                return Vector2.down;
            case PublicVars.Direction.up:
                return Vector2.up;
            case PublicVars.Direction.left:
                return Vector2.left;
            case PublicVars.Direction.right:
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
            case PublicVars.Direction.down:
                myDirection = PublicVars.Direction.up;
                transform.eulerAngles = new Vector3(0, 0, 180f);
                break;
            case PublicVars.Direction.up:
                myDirection = PublicVars.Direction.down;
                transform.eulerAngles = new Vector3(0, 0, 0f);
                break;
            case PublicVars.Direction.right:
                myDirection = PublicVars.Direction.left;
                transform.eulerAngles = new Vector3(0, 0, -90f);
                break;
            case PublicVars.Direction.left:
                myDirection = PublicVars.Direction.right;
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

    public PublicVars.Color GetMyColor()
    {
        return myColor;
    }
    public PublicVars.Direction GetMyDirection()
    {
        return myDirection;
    }

    
}
