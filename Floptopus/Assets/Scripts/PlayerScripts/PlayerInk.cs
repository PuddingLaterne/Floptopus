using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInk : MonoBehaviour
{
    public static PlayerInk instance;
    public float maxInk = 100;
    public float inkRegenaration = 10;
    float currentInk;
    Slider inkSlider ;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        currentInk = maxInk;
         inkSlider = GameObject.FindGameObjectWithTag("PlayerInkUI").GetComponent<Slider>();
	}

    public void CollectInk(float amount)
    {
        currentInk += amount;
    }

    public bool UseInk(float amount)
    {
        if (currentInk - amount > 0)
        {
            currentInk -= amount;
            return true;
        }
        else
            return false;
    }

	void Update () 
    {
        currentInk += Time.deltaTime * inkRegenaration;
        if (currentInk > maxInk)
            currentInk = maxInk;
        inkSlider.value = Mathf.Lerp(inkSlider.value, currentInk / maxInk, Time.deltaTime * 10);
	}
}