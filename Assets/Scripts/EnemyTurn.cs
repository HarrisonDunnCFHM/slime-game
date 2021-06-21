using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : MonoBehaviour
{

    //cached ref
    Hazard myHazard;

    private void Start()
    {
        myHazard = GetComponentInParent<Hazard>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Wall")
        {
            myHazard.ReverseDirection();
        }
        if (collision.GetComponent<Hazard>() != null && collision.name != "Slime Pool(Clone)")
        {
            myHazard.ReverseDirection();
        }
    }
}
