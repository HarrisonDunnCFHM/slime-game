using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider effectsSlider;

    float musicVolume;
    float effectsVolume;
    AudioSource myAudioSource;


    private void Awake()
    {
              int numberOfManagers = FindObjectsOfType<AudioManager>().Length;
        if (numberOfManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myAudioSource = FindObjectOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        musicVolume = musicSlider.value;
        effectsVolume = effectsSlider.value;
        myAudioSource.volume = musicVolume;
    }

    public float GetSFXVolume()
    {
        return effectsVolume;
    }
}
