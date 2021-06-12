using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{

    //config params
    [SerializeField] string slimeColor;
    [SerializeField] float slimeSpeed = 3;
    [SerializeField] float slimeMovesBase = 1f;

    Slime[] slimesOnMap;
    Hazard[] hazardsOnMap;

    //cached references
    bool activeSlime; 
    LevelManager levelManager;
    int enemyMoves = 0;
    Vector2 direction;
    float slimeMoves;
    Rigidbody2D myRigidbody;
    NextMove myNextMove;
    bool showMoves;
    Vector3? oldPos;

    // Start is called before the first frame update
    void Start()
    {
        activeSlime = false;
        slimesOnMap = FindObjectsOfType<Slime>();
        hazardsOnMap = FindObjectsOfType<Hazard>();
        levelManager = FindObjectOfType<LevelManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myNextMove = GetComponent<NextMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activeSlime)
        {
            HandleMovement();
        }
    }

    private IEnumerator ActivateHazards()
    {
        yield return new WaitForSeconds(.1f);
        foreach (Hazard hazard in hazardsOnMap)
        {
            hazard.ActivateHazard(slimeColor);
        }
    }

    private void OnMouseDown()
    {
        foreach (Slime slime in slimesOnMap)
        {
            slime.activeSlime = false;
            slime.myNextMove.DeleteOldMoves();
        }
        activeSlime = true;
        showMoves = true;

    }

    private void HandleMovement()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            showMoves = true;
            direction = Vector2.up;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            showMoves = true;
            direction = Vector2.down;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            showMoves = true;
            direction = Vector2.right;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            showMoves = true;
            direction = Vector2.left;
            slimeMoves = slimeMovesBase;
            enemyMoves++;
        }
        if (slimeMoves > 0)
        {
            if (oldPos == null)
            {
                oldPos = transform.position;
                Debug.Log("oldPos stored: " + oldPos.Value.ToString());
            }
            myRigidbody.velocity = direction * slimeSpeed;
            slimeMoves -= Time.deltaTime * slimeSpeed;
        }
        else if (slimeMoves <= 0)
        {
            myRigidbody.velocity = Vector2.zero;
            SnapToGrid();
            if (showMoves == true)
            {
                oldPos = null;
                myNextMove.CheckForNextMoves(slimeColor);
                showMoves = false;
            }
        }
        if (enemyMoves > 0)
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
        HazardCheck(collision);
        GoalCheck(collision);
        WallCheck(collision);
    }

    private void WallCheck(Collider2D collision)
    {
        var collidedWall = collision.name;
        if (collidedWall == "Wall")
        {
            transform.position = oldPos.Value;
            Debug.Log("pos set to oldpos " + oldPos.Value.ToString());
            oldPos = null;

            /*var direction = transform.position - collision.transform.position;
            int roundedX = Mathf.RoundToInt(direction.x);
            int roundedY = Mathf.RoundToInt(direction.y);
            Vector3 roundedDir = new Vector2(roundedX, roundedY);
            transform.position -= roundedDir.normalized;*/
        }
    }

    private void GoalCheck(Collider2D collision)
    {
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == slimeColor)
            {
                collidedGoal.ActivateGoal();
                levelManager.OnGoal();
            }
        }
    }

    private void HazardCheck(Collider2D collision)
    {
        var collidedHazard = collision.GetComponent<Hazard>();
        if (collidedHazard != null)
        {
            levelManager.ResetLevel();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == slimeColor)
            {
                collidedGoal.DeactivateGoal();
                levelManager.OffGoal();
            }
        }
    }

    public string GetSlimeColor()
    {
        return slimeColor;
    }

}
