using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //config params
    [SerializeField] PublicVars.Color myColor;
    
    //bool goalActive;


    //cached ref
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        //goalActive = false;
    }

    /*public void ActivateGoal()
    {
        goalActive = true;
    }

    public void DeactivateGoal()
    {
        goalActive = false;
    }*/

    public PublicVars.Color GetGoalColor()
    {
        return myColor;
    }

}
