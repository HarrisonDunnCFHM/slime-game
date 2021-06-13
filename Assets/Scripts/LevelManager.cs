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
    [SerializeField] GameObject resetMenu;
    [SerializeField] List<AudioClip> slimeWin;
    [SerializeField] float slimeVolume = 0.6f;


    int goalsNeeded;
    int goalsHave;
    

    //cached refs
    Goal[] goalsOnMap;
    bool levelWin;

    private void Start()
    {
        goalsOnMap = FindObjectsOfType<Goal>();
        goalsNeeded = goalsOnMap.Length;
        goalsHave = 0;
        goalsFinished.text = "Goals: " + goalsHave.ToString() + "/" + goalsNeeded.ToString();
        winScreen.SetActive(false);
        resetMenu.SetActive(false);
        levelWin = false;
    }

    private void Update()
    {
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
        AudioSource.PlayClipAtPoint(clipSet[pickedSound], Camera.main.transform.position, slimeVolume);
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
    
    public void GameOver()
    {
        resetMenu.SetActive(true);
    }
    
    public void ResetLevel()
    {
        resetMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
