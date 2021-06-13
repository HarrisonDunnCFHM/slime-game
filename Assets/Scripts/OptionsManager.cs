using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    [SerializeField] bool colorBlindOn;
    [SerializeField] GameObject optionsMenu;


    private void Awake()
    {
        int optionsManagers = FindObjectsOfType<OptionsManager>().Length;
        if (optionsManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }

    private void Start()
    {
        optionsMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOptions();
        }
    }

    public void ToggleColorBlind()
    {
        if (colorBlindOn) { colorBlindOn = false; Debug.Log("colorblind disabled"); }
        else if (!colorBlindOn) { colorBlindOn = true; Debug.Log("colorblind enabled"); }
    }

    public bool CheckColorBlind()
    {
        return colorBlindOn;
    }


    public void ToggleOptions()
    {
        if (optionsMenu.activeSelf) { optionsMenu.SetActive(false); }
        else if (!optionsMenu.activeSelf) { optionsMenu.SetActive(true); }
    }

    
}
