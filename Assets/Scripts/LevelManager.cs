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
    [SerializeField] GameObject retryMenu;
    [SerializeField] List<AudioClip> slimeWin;
    [SerializeField] float slimeVolume = 0.6f;
    [SerializeField] bool isSplashScreen;


    int goalsNeeded;
    int goalsHave;
    

    //cached refs
    Goal[] goalsOnMap;
    bool levelWin;
    bool levelLose;

    private void Start()
    {
        if (isSplashScreen) { return; }
        goalsOnMap = FindObjectsOfType<Goal>();
        goalsNeeded = goalsOnMap.Length;
        goalsHave = 0;
        goalsFinished.text = "Flags: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
        winScreen.SetActive(false);
        retryMenu.SetActive(false);
        levelWin = false;
    }

    private void Update()
    {
        if (isSplashScreen) { return; }

        if (goalsHave == goalsNeeded)
        {
            if (!levelWin)
            {
                Slime[] activeSlimes = FindObjectsOfType<Slime>();
                foreach (Slime slime in activeSlimes)
                {
                    playSound(slimeWin);
                }
                winScreen.SetActive(true);
                levelWin = true;
            }
        }
    }

    private void playSound(List<AudioClip> clipSet)
    {
        int pickedSound = UnityEngine.Random.Range(0, clipSet.Count);
        AudioSource.PlayClipAtPoint(clipSet[pickedSound], Camera.main.transform.position, slimeVolume + 0.2f);
    }

    public void ResetGoals()
    {
        goalsHave = 0;
        goalsFinished.text = "Flags: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
    }

    public void OnGoal()
    {
        goalsHave++;
        goalsFinished.text = "Flags: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
    }

    public void OffGoal()
    {
        goalsHave--;
        goalsFinished.text = "Flags: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
    }
    
    public void GameOver()
    {
        levelLose = true;
        retryMenu.SetActive(true);
    }
    
    public void ResetLevel()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelLose = false;
        levelWin = false;
        ResetGoals();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelWin = false;
        ResetGoals();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool CheckLevelPlayable()
    {
        if (levelLose || levelWin)
        {
            return false;
        }
        else
        { 
            return true; 
        }
    }

    public void LoadMainMenu()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelWin = false;
        levelLose = false;
        ResetGoals();
        SceneManager.LoadScene(0);
    }

}
