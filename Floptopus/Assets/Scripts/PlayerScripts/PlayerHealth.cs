using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour 
{
    public float maxHealth = 100;
    float currentHealth;
    Slider healthSlider ;
	void Start () 
    {
        currentHealth = maxHealth;
        healthSlider = GameObject.FindGameObjectWithTag("PlayerHealthUI").GetComponent<Slider>();
	}

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Application.LoadLevel(0);
    }

	void Update () 
    {
        healthSlider.value = currentHealth / maxHealth;
	}
}
