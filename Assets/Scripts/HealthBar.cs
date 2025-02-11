﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public int maxHealth;                   //initialize Max health
    public int currentHealth;                //Initialize current health
    public int health = 100;

    public void takeDamage(int damage)
    {
        slider.value -= damage;
    }

    public void getHealth(int heal)
    {
        slider.value += heal;
    }
    
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
    }
    
    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
