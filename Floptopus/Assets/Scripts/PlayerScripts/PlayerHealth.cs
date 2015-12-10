﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour 
{
    public static PlayerHealth instance;
    public float maxHealth = 100;
    float currentHealth;
    Slider healthSlider ;

    void Awake()
    {
        instance = this;
    }

	void Start () 
    {
        currentHealth = maxHealth;
        healthSlider = GameObject.FindGameObjectWithTag("PlayerHealthUI").GetComponent<Slider>();
	}

    public void CollectHealth(float value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Application.LoadLevel(0);
    }

	void Update () 
    {
        healthSlider.value = Mathf.Lerp(healthSlider.value, currentHealth / maxHealth, Time.deltaTime * 10);
	}
}
