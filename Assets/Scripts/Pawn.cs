using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour
{

    //config params
    [SerializeField] string pawnColor;
    [SerializeField] float slimeSpeed = 3;
    [SerializeField] float slimeMovesBase = 1f;

    Pawn[] pawnsOnMap;
    Hazard[] hazardsOnMap;

    //cached references
    bool activePawn; 
    LevelManager levelManager;
    int enemyMoves = 0;
    Vector2 direction;
    float slimeMoves;
    Rigidbody2D myRigidbody;
 
    // Start is called before the first frame update
    void Start()
    {
        activePawn = false;
        pawnsOnMap = FindObjectsOfType<Pawn>();
        hazardsOnMap = FindObjectsOfType<Hazard>();
        levelManager = FindObjectOfType<LevelManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activePawn)
        {
            HandleMovement();
        }
    }

    private IEnumerator ActivateHazards()
    {
        yield return new WaitForSeconds(.1f);
        foreach (Hazard hazard in hazardsOnMap)
        {
            hazard.ActivateHazard(pawnColor);
        }
    }

    private void OnMouseDown()
    {
        foreach (Pawn pawn in pawnsOnMap)
        {
            pawn.activePawn = false;
        }
        activePawn = true;
    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            direction = Vector2.up;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            direction = Vector2.down;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            direction = Vector2.right;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            direction = Vector2.left;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        if (slimeMoves > 0)
        {
            myRigidbody.velocity = direction * slimeSpeed;
            slimeMoves -= Time.deltaTime * slimeSpeed;
        }
        else if (slimeMoves <= 0)
        {
            myRigidbody.velocity = Vector2.zero;
            SnapToGrid();
        }
        if(enemyMoves > 0)
        {
            StartCoroutine(ActivateHazards());
            enemyMoves--;
        }
    }
    private void SnapToGrid()
    {
        float newX = Mathf.RoundToInt(transform.position.x);
        float newY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector2(newX, newY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collidedHazard = collision.GetComponent<Hazard>();
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedHazard != null)
        {
                levelManager.ResetLevel();
        }
        else if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == pawnColor)
            {
                collidedGoal.ActivateGoal();
                levelManager.OnGoal();
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == pawnColor)
            {
                collidedGoal.DeactivateGoal();
                levelManager.OffGoal();
            }
        }
    }

    public string GetPawnColor()
    {
        return pawnColor;
    }

}
