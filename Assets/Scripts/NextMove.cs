using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextMove : MonoBehaviour
{
    //config params
    [SerializeField] GameObject nextMoveTarget;

    //cached refs
    Vector3[] directions;
    List<GameObject> oldMoves;
    Vector2 nextUp;
    Vector2 nextDown;
    Vector2 nextRight;
    Vector2 nextLeft;

    // Start is called before the first frame update
    void Start()
    {
        directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.right, Vector3.left};
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForNextMoves(string slimeColor, GameObject slime)
    {
        DeleteOldMoves();
        oldMoves = new List<GameObject>();
        float slimeDist;
        if (slimeColor == "blue")
        {
            slimeDist = Mathf.Infinity;
        }
        else
        {
            slimeDist = 1f;
        }
        foreach (Vector3 direction in directions)
        {
            int wallLayer = LayerMask.GetMask("Walls");
            int slimeLayer = LayerMask.GetMask("Slimes");
            int layerMask = slimeLayer | wallLayer;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, slimeDist, layerMask);
            if (!hit)
            {
                var newMove = Instantiate(nextMoveTarget, transform.position + direction, Quaternion.identity);
                oldMoves.Add(newMove);
            }
            else if (hit && slimeColor == "blue" && hit.collider.name == "Wall")
            {
                Vector2 transform2d = transform.position;
                var hitVector2 = transform2d - hit.point;
                float targetDistance = hitVector2.magnitude - 0.5f;
                if (targetDistance != 0)
                {
                    Vector3 newTarget = hitVector2.normalized * targetDistance;
                    var newMove = Instantiate(nextMoveTarget, transform.position - newTarget, Quaternion.identity);
                    oldMoves.Add(newMove);
                }
                else
                {
                    oldMoves.Add(null);
                }
            }
            else if (hit && slimeColor == "blue" && hit.collider.GetComponent<Slime>() != null)
            {
                var hitVector3 = transform.position - hit.collider.transform.position;
                var targetDistance = hitVector3.magnitude - 1;
                if (targetDistance != 0)
                {
                    var newTarget = hitVector3.normalized * targetDistance;
                    var newMove = Instantiate(nextMoveTarget, transform.position - newTarget, Quaternion.identity);
                    oldMoves.Add(newMove);
                }
                else
                {
                    oldMoves.Add(null);
                }
            }
            else if (hit && slimeColor == "blue")
            {

                var newMove = Instantiate(nextMoveTarget, hit.collider.transform.position, Quaternion.identity);
                oldMoves.Add(newMove);
            }
            else if (hit && slimeColor == "red" && hit.collider.GetComponent<Slime>() != null)
            {
                RaycastHit2D secondHit = Physics2D.Raycast(hit.collider.GetComponent<Rigidbody2D>().position, direction, slimeDist, layerMask);
                if (!secondHit)
                {
                    var newMove = Instantiate(nextMoveTarget, hit.collider.transform.position, Quaternion.identity);
                    oldMoves.Add(newMove);
                }
                else
                {
                    oldMoves.Add(null);
                }
            }
            else if (hit)
            {
                oldMoves.Add(null);
            }
        }
        if (oldMoves[0] != null)
        { 
            nextUp = oldMoves[0].transform.position;
        }
        else { nextUp = slime.transform.position; }
        if (oldMoves[1] != null)
        {
            nextDown = oldMoves[1].transform.position;
        }
        else { nextDown = slime.transform.position; }
        if (oldMoves[2] != null)
        {
            nextRight = oldMoves[2].transform.position;
        }
        else { nextRight = slime.transform.position; }
        if (oldMoves[3] != null)
        {
            nextLeft = oldMoves[3].transform.position;
        } 
        else { nextLeft = slime.transform.position; }
        slime.GetComponent<Slime>().GatherNextMoves(nextUp, nextDown, nextRight, nextLeft);
    }

    public void ShowNextMove()
    {
        DeleteOldMoves();
        oldMoves = new List<GameObject>();
        foreach (Vector3 direction in directions)
        {
            var newMove = Instantiate(nextMoveTarget, transform.position + direction, Quaternion.identity);
            oldMoves.Add(newMove);
        }
    }

    public void DeleteOldMoves()
    {
        if (oldMoves == null) { return; }
        foreach (GameObject move in oldMoves)
        {
            Destroy(move);
        }
    }
}
