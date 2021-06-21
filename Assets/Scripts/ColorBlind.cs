using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBlind : MonoBehaviour
{
    //config parameters
    [SerializeField] Sprite mySprite;

    //cached references
    OptionsManager optionsManager;

    // Start is called before the first frame update
    void Start()
    {
        optionsManager = FindObjectOfType<OptionsManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (optionsManager.CheckColorBlind())
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = mySprite;
        }
        else if (!optionsManager.CheckColorBlind())
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
    }

    public void colorBlindEnable()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = mySprite;
    }
    public void colorBlindDisable()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = null;
    }
}
