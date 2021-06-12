using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    //config params
    [SerializeField] string goalColor;
    
    bool goalActive;


    //cached ref
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        goalActive = false;
    }

    public void ActivateGoal()
    {
        goalActive = true;
    }

    public void DeactivateGoal()
    {
        goalActive = false;
    }

    public string GetGoalColor()
    {
        return goalColor;
    }

}
