using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{

    //config params
    [Header("General Slime Config")]
    [Tooltip("Color of the slime")] [SerializeField] PublicVars.Color myColor;
    [Tooltip("Speed of moving to next tile")] [SerializeField] float slimeSpeed = 3;
    [Tooltip("Time to reach next time")] [SerializeField] float spacesToMove = 1f; //number of spaces to move, works as a timer with float slimeMovesTimer;
    float slimeMovesTimer;

    [Header("Green Slime Options")]
    [SerializeField] GameObject slimePool;
    [SerializeField] Vector3 poolShrinkSpeed = new Vector3(.2f, .2f, .2f);
    [SerializeField] int maxSlimePools;
    [SerializeField] string SLIMECLONENAME = "Slime Pool(Clone)";

    [Header("Slime Sounds")]
    [SerializeField] List<AudioClip> slimeSelect;
    [SerializeField] List<AudioClip> slimeSquish;
    [SerializeField] List<AudioClip> slimeDeath;
    [SerializeField] float selectVolOffset = 0.2f; //recording quality for select is a little low
    [SerializeField] float squishVolOffset = -0.1f; //recording qualify for squish is a little high

    [SerializeField] float minMaskSize = 8f;

    //cached references
    LevelManager levelManager;
    AudioManager audioManager;
    Animator myAnimator;
    Slime[] slimesOnMap;
    Hazard[] hazardsOnMap;
    Queue<GameObject> poolsOnMap;
    NextMove myNextMove;
    Rigidbody2D myRigidbody;
    GameObject myGameOverMask;
    Vector2 direction;
    Vector3? oldPos;
    Vector3 nextUp;
    Vector3 nextDown;
    Vector3 nextRight;
    Vector3 nextLeft;
    bool activeSlime;
    bool showMoves;
    bool pushed;
    bool canPickSlimes;
    [SerializeField] bool isDead;
    int enemyMoveCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        slimesOnMap = FindObjectsOfType<Slime>();
        hazardsOnMap = FindObjectsOfType<Hazard>();
        levelManager = FindObjectOfType<LevelManager>();
        audioManager = FindObjectOfType<AudioManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myNextMove = GetComponent<NextMove>();
        myAnimator = GetComponent<Animator>();
        myGameOverMask = GetComponentInChildren<SpriteMask>().gameObject;
        myGameOverMask.SetActive(false);
        poolsOnMap = new Queue<GameObject>();
        activeSlime = false;
        pushed = false;
        canPickSlimes = true;
        isDead = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (activeSlime)
        {
            HandleMovement();
        }
        LayerGrid();
        ShrinkGameOverMask();
    }

    private void ShrinkGameOverMask()
    {
        if (isDead)
        {
            myGameOverMask.SetActive(true);
            if (myGameOverMask.transform.localScale.x > minMaskSize)
            {
                myGameOverMask.transform.localScale = new Vector3
                (myGameOverMask.transform.localScale.x - (Time.deltaTime * (levelManager.GetWipeSpeed())),
                myGameOverMask.transform.localScale.y - (Time.deltaTime * (levelManager.GetWipeSpeed())), 1f);
            }
            else if (myGameOverMask.transform.localScale.x <= minMaskSize)
            {
                myGameOverMask.transform.localScale = new Vector3(minMaskSize, minMaskSize, 1f);
            }
        }
    }

    private void LayerGrid()
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y, transform.position.y - 10);
    }

    private void ActivateHazards()
    {
        foreach (Hazard hazard in hazardsOnMap)
        {
            hazard.ActivateHazard(myColor);
        }
    }

    private void OnMouseDown()
    {
        if (!levelManager.CheckLevelPlayable()) { return; }
        if (!canPickSlimes) { return; }
        foreach (Slime slime in slimesOnMap)
        {
            if (!slime.canPickSlimes) { return; }
            slime.activeSlime = false;
            slime.myNextMove.DeleteOldMoves();
        }
        activeSlime = true;
        showMoves = true;
        pushed = false;
        PlaySound(slimeSelect, selectVolOffset);
        levelManager.SetActiveColor(myColor);
    }

    private void HandleMovement()
    {
        if (myRigidbody.velocity == Vector2.zero)
        {
            if (!levelManager.CheckLevelPlayable()) { return; }
            if (slimeMovesTimer <= 0)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    if (nextUp == transform.position) { return; }
                    canPickSlimes = false;
                    showMoves = true;
                    direction = nextUp - transform.position;
                    slimeMovesTimer = spacesToMove;
                    enemyMoveCounter++;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    if (nextDown == transform.position) { return; }
                    canPickSlimes = false;
                    showMoves = true;
                    direction = nextDown - transform.position;
                    slimeMovesTimer = spacesToMove;
                    enemyMoveCounter++;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    if (nextRight == transform.position) { return; }
                    canPickSlimes = false;
                    showMoves = true;
                    direction = nextRight - transform.position;
                    slimeMovesTimer = spacesToMove;
                    enemyMoveCounter++;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    if (nextLeft == transform.position) { return; }
                    canPickSlimes = false;
                    showMoves = true;
                    direction = nextLeft - transform.position;
                    slimeMovesTimer = spacesToMove;
                    enemyMoveCounter++;
                }
            }
        }
        if (slimeMovesTimer > 0)
        {
            if (oldPos == null)
            {
                PlaySound(slimeSquish, squishVolOffset);
                oldPos = transform.position;
                if (slimePool != null)//green slime poison rules
                {
                    GameObject newPool = Instantiate(slimePool, oldPos.Value, Quaternion.identity);
                    if (poolsOnMap.Count < maxSlimePools)
                    {
                        poolsOnMap.Enqueue(newPool);
                    }
                    else
                    {
                        Destroy(poolsOnMap.Dequeue());
                        poolsOnMap.Enqueue(newPool);
                    }
                    foreach (GameObject pool in poolsOnMap)
                    {
                        pool.transform.localScale -= poolShrinkSpeed;
                    }
                }
            }
            transform.Translate(direction.normalized * slimeSpeed * Time.deltaTime);
            
            //myRigidbody.velocity = direction.normalized * slimeSpeed;
            slimeMovesTimer -= Time.deltaTime * slimeSpeed / direction.magnitude;
            //Vector2 distToTarget = direction - myRigidbody.position; //I can't remember what this does anymore. Just leaving it here for now
        }
        else if (slimeMovesTimer <= 0)
        {
            canPickSlimes = true;
            SnapToGrid();
            myRigidbody.velocity = Vector2.zero;
            myNextMove.CheckForNextMoves(myColor, gameObject);
            if (showMoves == true)
            {
                oldPos = null;
                showMoves = false;
            }
        }
        if (enemyMoveCounter > 0)
        {
            ActivateHazards();
            enemyMoveCounter--;
        }
    }

    public void GatherNextMoves(Vector2 nextU, Vector2 nextD, Vector2 nextR, Vector2 nextL)
    {
        nextUp = nextU;
        nextDown = nextD;
        nextRight = nextR;
        nextLeft = nextL;
    }

    private void SnapToGrid()
    {
        float newX = Mathf.RoundToInt(transform.position.x);
        float newY = Mathf.RoundToInt(transform.position.y);
        myRigidbody.position = new Vector2(newX, newY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FearCheck(collision);
        HazardCheck(collision);
        GoalCheck(collision);
        SlimeCheck(collision);
        WallCheck(collision);
    }



    private void FearCheck(Collider2D collision)
    {
        var inFrontOfCrab = collision.GetComponent<EnemyTurn>();
        if (inFrontOfCrab != null)
        {
            if (myAnimator.GetBool("isAfraid") == false)
            {
                myAnimator.SetBool("isAfraid", true);
            }
            else
            {
                myAnimator.SetBool("isAfraid", false);
            }
        }
    }

    private void SlimeCheck(Collider2D collision)
    {
        if (myColor != PublicVars.Color.red) { return; }
        var collidedSlime = collision.GetComponent<Slime>();
        if (collidedSlime != null)
        {
            if (collidedSlime.activeSlime == true) { return; }
            collidedSlime.pushed = true;
            var collidedSlimePos = collidedSlime.myRigidbody.transform.position;
            var collidedDirection = collidedSlimePos - myRigidbody.transform.position;
            var collidedDirNoZ = new Vector3(collidedDirection.x, collidedDirection.y, 0);
            collidedSlime.myRigidbody.transform.position += collidedDirNoZ.normalized;
            collidedSlime.SnapToGrid();
        }
    }

    private void WallCheck(Collider2D collision)
    {
        var collidedWall = collision.name;
        if (collidedWall == "Wall")
        {
            if (pushed)
            {
                //levelManager.ResetLevel(); //disabled feature - leaving in for now just in case
            }
        }
    }

    private void GoalCheck(Collider2D collision)
    {
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == myColor)
            {
                levelManager.OnGoal();
            }
        }
    }

    public bool CheckDead()
    {
        return isDead;
    }

    private void HazardCheck(Collider2D collision)
    {
        if (isDead) { return; }
        var collidedHazard = collision.GetComponent<Hazard>();
        if (collidedHazard != null)
        {
            if (myColor != PublicVars.Color.green)
            {
                myRigidbody.velocity = Vector2.zero;
                collidedHazard.StopHazardMove();
                PlaySound(slimeDeath, 0.1f);
                isDead = true;
                levelManager.GameOver();
            }
            else
            {
                if (collidedHazard.name == SLIMECLONENAME)
                {
                    return;
                }
                else
                {
                    myRigidbody.velocity = Vector2.zero;
                    PlaySound(slimeDeath, 0.1f);
                    collidedHazard.StopHazardMove();
                    isDead = true;
                    levelManager.GameOver();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        FearCheck(collision);
        var collidedGoal = collision.GetComponent<Goal>();
        if (collidedGoal != null)
        {
            var goalColor = collidedGoal.GetGoalColor();
            if (goalColor == myColor)
            {
                levelManager.OffGoal();
            }
        }
    }

    private void PlaySound(List<AudioClip> clipSet, float volumeOffset)
    {
        int pickedSound = UnityEngine.Random.Range(0, clipSet.Count);
        AudioSource.PlayClipAtPoint(clipSet[pickedSound], Camera.main.transform.position, GetVolume() + volumeOffset);
    }

    private float GetVolume()
    {
        float currentVol = audioManager.GetSFXVolume();
        return currentVol;
    }
    public PublicVars.Color GetSlimeColor()
    {
        return myColor;
    }


}
