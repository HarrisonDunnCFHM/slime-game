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
    [SerializeField] GameObject slimePool;
    [SerializeField] Vector3 poolShrinkSpeed = new Vector3(.2f, .2f, .2f);
    [SerializeField] int maxSlimePools;
    [SerializeField] List<AudioClip> slimeSelect;
    [SerializeField] List<AudioClip> slimeSquish;
    [SerializeField] List<AudioClip> slimeDeath;

    Slime[] slimesOnMap;
    Hazard[] hazardsOnMap;
    Queue<GameObject> poolsOnMap;

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
    Vector3 nextUp;
    Vector3 nextDown;
    Vector3 nextRight;
    Vector3 nextLeft;
    bool pushed;
    [SerializeField] bool canPickSlimes;
    AudioManager audioManager;


    // Start is called before the first frame update
    void Start()
    {
        activeSlime = false;
        slimesOnMap = FindObjectsOfType<Slime>();
        hazardsOnMap = FindObjectsOfType<Hazard>();
        levelManager = FindObjectOfType<LevelManager>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myNextMove = GetComponent<NextMove>();
        poolsOnMap = new Queue<GameObject>();
        pushed = false;
        audioManager = FindObjectOfType<AudioManager>();
        //bool canPickSlimes = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (activeSlime)
        {
            HandleMovement();
        }
        else
        {
            //SnapToGrid();
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
        PlaySound(slimeSelect);
        PlaySound(slimeSquish); 
    }

    private void HandleMovement()
    {
        if (myRigidbody.velocity == Vector2.zero)
        {
            if (!levelManager.CheckLevelPlayable()) { return; }
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (nextUp == transform.position) { return; }
                canPickSlimes = false;
                showMoves = true;
                direction = nextUp - transform.position;
                slimeMoves = slimeMovesBase;
                enemyMoves++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (nextDown == transform.position) { return; }
                canPickSlimes = false;
                showMoves = true;
                direction = nextDown - transform.position;
                slimeMoves = slimeMovesBase;
                enemyMoves++;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                if (nextRight == transform.position) { return; }
                canPickSlimes = false;
                showMoves = true;
                direction = nextRight - transform.position;
                slimeMoves = slimeMovesBase;
                enemyMoves++;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                if (nextLeft == transform.position) { return; }
                canPickSlimes = false;
                showMoves = true;
                direction = nextLeft - transform.position;
                slimeMoves = slimeMovesBase;
                enemyMoves++;
            }
        }
        if (slimeMoves > 0)
        {
            if (oldPos == null) 
            {
                PlaySound(slimeSquish);
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
            myRigidbody.velocity = direction.normalized * slimeSpeed;
            slimeMoves -= Time.deltaTime * slimeSpeed / direction.magnitude;
            Vector2 distToTarget = direction - myRigidbody.position;
        }
        else if (slimeMoves <= 0)
        {
            canPickSlimes = true;
            SnapToGrid();
            myRigidbody.velocity = Vector2.zero;
            myNextMove.CheckForNextMoves(slimeColor, gameObject);

            if (showMoves == true)
            {
                oldPos = null;
                showMoves = false;
            }
        }
        if (enemyMoves > 0)
        {
            StartCoroutine(ActivateHazards());
            enemyMoves--;
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
        HazardCheck(collision);
        GoalCheck(collision);
        SlimeCheck(collision);
        WallCheck(collision);
    }

    private void SlimeCheck(Collider2D collision)
    {
        if (slimeColor != "red") { return; }
        var collidedSlime = collision.GetComponent<Slime>();
        if (collidedSlime != null)
        {
            collidedSlime.pushed = true;
            var collidedSlimePos = collidedSlime.myRigidbody.transform.position;
            var collidedDirection = collidedSlimePos - myRigidbody.transform.position;
            collidedSlime.myRigidbody.transform.position += collidedDirection.normalized;
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
                levelManager.ResetLevel();
            }
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
                //collidedGoal.ActivateGoal();
                levelManager.OnGoal();
            }
        }
    }

    private void HazardCheck(Collider2D collision)
    {
        var collidedHazard = collision.GetComponent<Hazard>();
        if (collidedHazard != null)
        {
            if (slimeColor != "green")
            {
                PlaySound(slimeDeath);
                levelManager.GameOver();
            }
            else
            {
                if (collidedHazard.name == "Slime Pool(Clone)")
                {
                    return;
                }
                else
                {
                    PlaySound(slimeDeath);
                    levelManager.GameOver();
                }
            }
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
                //collidedGoal.DeactivateGoal();
                levelManager.OffGoal();
            }
        }
    }

    private void PlaySound(List<AudioClip> clipSet)
    {
        int pickedSound = UnityEngine.Random.Range(0, clipSet.Count);
        AudioSource.PlayClipAtPoint(clipSet[pickedSound], Camera.main.transform.position, GetVolume());
    }

    private float GetVolume()
    {
        float currentVol = audioManager.GetSFXVolume();
        return currentVol;
    }
    public string GetSlimeColor()
    {
        return slimeColor;
    }

}
