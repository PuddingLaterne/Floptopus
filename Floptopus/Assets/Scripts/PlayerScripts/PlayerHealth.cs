using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour 
{
    public float maxHealth = 100;
    float currentHealth;
    Slider healthSlider ;
    Animator anim;
	void Start () 
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthSlider = GameObject.FindGameObjectWithTag("PlayerHealthUI").GetComponent<Slider>();
	}

    public void TakeDamage(float damage)
    {
        anim.SetTrigger("hurt");
        currentHealth -= damage;
        if (currentHealth <= 0)
            Application.LoadLevel(0);
    }

	void Update () 
    {
        healthSlider.value = currentHealth / maxHealth;
	}
}
