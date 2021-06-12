using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    //config params
    [SerializeField] Text goalsFinished;
    [SerializeField] GameObject winScreen;
    
    int goalsNeeded;
    int goalsHave;
    

    //cached refs
    Goal[] goalsOnMap;

    private void Start()
    {
        goalsOnMap = FindObjectsOfType<Goal>();
        goalsNeeded = goalsOnMap.Length;
        goalsHave = 0;
        goalsFinished.text = "Goals: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
        winScreen.SetActive(false);
    }

    private void Update()
    {
        if (goalsHave == goalsNeeded)
        {
            winScreen.SetActive(true);
        }
    }

    public void OnGoal()
    {
        goalsHave++;
        goalsFinished.text = "Goals: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
    }

    public void OffGoal()
    {
        goalsHave--;
        goalsFinished.text = "Goals: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
    }
    
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
