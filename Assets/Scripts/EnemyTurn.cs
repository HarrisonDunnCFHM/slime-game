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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        myHazard.ReverseDirection();
    }
}
