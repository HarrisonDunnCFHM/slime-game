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
    [SerializeField] float goalTimer = 1f;

    //cached refs
    Goal[] goalsOnMap;
    PublicVars.Color activeColor;
    bool levelPlayable;
    float currentGoalTime;
    int goalsNeeded;
    int goalsHave;

    private void Start()
    {
        if (isSplashScreen) { return; }
        goalsOnMap = FindObjectsOfType<Goal>();
        goalsNeeded = goalsOnMap.Length;
        goalsHave = 0;
        goalsFinished.text = "Flags: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
        winScreen.SetActive(false);
        retryMenu.SetActive(false);
        levelPlayable = true;
    }

    private void Update()
    {
        if (isSplashScreen) { return; }
        if (goalsHave == goalsNeeded)
        {
            if (levelPlayable)
            {
                currentGoalTime -= Time.deltaTime;
                if (currentGoalTime <= 0)
                {
                    Slime[] activeSlimes = FindObjectsOfType<Slime>();
                    foreach (Slime slime in activeSlimes)
                    {
                        PlaySound(slimeWin);
                    }
                    winScreen.SetActive(true);
                    levelPlayable = false;
                }
            }
        }
        else
        {
            currentGoalTime = goalTimer;
        }
    }

    private void PlaySound(List<AudioClip> clipSet)
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
        levelPlayable = false;
        retryMenu.SetActive(true);
    }
    
    public void ResetLevel()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelPlayable = true;
        ResetGoals();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelPlayable = true;
        ResetGoals();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool CheckLevelPlayable()
    {
        return levelPlayable;
    }

    public void LoadMainMenu()
    {
        retryMenu.SetActive(false);
        winScreen.SetActive(false);
        levelPlayable = true;
        ResetGoals();
        SceneManager.LoadScene(0);
    }

    public void SetActiveColor(PublicVars.Color slimeColor)
    {
        activeColor = slimeColor;
    }

    public PublicVars.Color GetActiveColor()
    {
        return activeColor;
    }
}
