using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : MonoBehaviour
{

    //cached ref
    Hazard myHazard;
    LevelManager levelManager;

    private void Start()
    {
        myHazard = GetComponentInParent<Hazard>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PublicVars.Color myColor = GetComponentInParent<Hazard>().GetMyColor();
        PublicVars.Direction myDirection = GetComponentInParent<Hazard>().GetMyDirection();
        
        if (collision.name == "Wall")
        {
            myHazard.ReverseDirection();
            return;
        }
        if (collision.GetComponent<Hazard>() != null && collision.name != "Slime Pool(Clone)")
        {
            PublicVars.Color collidedColor = collision.GetComponent<Hazard>().GetMyColor();
            PublicVars.Direction collidedDirection = collision.GetComponent<Hazard>().GetMyDirection(); 
            if (myColor == collidedColor) //prevents same colored crabs from blocking eachothers movements if they wouldn't occupy same space after move
            {
                if ((myDirection == PublicVars.Direction.up && collidedDirection != PublicVars.Direction.down) ||
                    (myDirection == PublicVars.Direction.down && collidedDirection != PublicVars.Direction.up) ||
                    (myDirection == PublicVars.Direction.left && collidedDirection != PublicVars.Direction.right) ||
                    (myDirection == PublicVars.Direction.right && collidedDirection != PublicVars.Direction.left))
                {
                    return;
                }
            }
            if (collidedColor == levelManager.GetActiveColor()) //prevents inactive crabs from flipping from active crabs colliding
            {
                return;
            }
            myHazard.ReverseDirection();
        }
    }



}
