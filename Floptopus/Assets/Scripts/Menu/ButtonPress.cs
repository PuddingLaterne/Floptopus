using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPress : MonoBehaviour 
{
    enum State
    {
        main,
        howToPlay,
        credits
    };

    public GameObject overlay;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    public GameObject [] menuStates;
    State currentState;

    AudioSource audio;

    public void Start()
    {
        menuStates[0].SetActive(true);
        for (int i = 1; i < menuStates.Length; i++)
        {
            menuStates[i].SetActive(false);
        }
        currentState = State.main;

        audio = GetComponent<AudioSource>();
    }

    public void Play()
    {
        StartCoroutine(LoadScene(1, 0.2f));
    }

    public void LoadSubmenu(int index)
    {
        menuStates[(int)currentState].SetActive(false);
        currentState = (State)index;
        menuStates[(int)currentState].SetActive(true);
    }

    public void BackToMenu()
    {
        if (Application.loadedLevel != 0)
        {
            StartCoroutine(LoadScene(0, 0.2f));
        }
        else
        {
            menuStates[(int)currentState].SetActive(false);
        }
    }

    public void Hover()
    {
        if (!audio.isPlaying)
        {
            audio.clip = hoverSound;
            audio.Play();
        }
    }

    public void Click()
    {
        audio.clip = clickSound;
        audio.Play();
    }

    IEnumerator LoadScene(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        Application.LoadLevel(index);
    }

    public void Close()
    {
        Application.Quit();
        Debug.Log("close");
    }
}
