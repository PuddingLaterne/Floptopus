using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonPress : MonoBehaviour 
{
    public GameObject overlay;

    public void Play()
    {
        overlay.GetComponent<Overlay>().FadeOut();
        StartCoroutine(LoadScene(1, 0.2f));
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
