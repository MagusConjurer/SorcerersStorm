using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss
{
    private int startingHealth;
    private bool onTheBoard;
    private int currentHealth;
    private string bossName;
    /// <summary>
    /// Contains the data for a boss
    /// </summary>
    /// <param name="name">Title to display on the boss panel</param>
    /// <param name="health">Starting health of the boss</param>
    public Boss(string name, int health)
    {
        onTheBoard = false;
        bossName = name;
        startingHealth = health;
    }
    public string GetBossName()
    {
        return bossName;
    }

    public int GetBossHealth()
    {
        return currentHealth;
    }

    /// <returns>True if boss health > 0</returns>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public bool IsOnTheBoard()
    {
        return onTheBoard;
    }

    /// <summary>
    /// Method to decrease the boss health by a given amount.
    /// </summary>
    /// <param name="amount">Damage to apply</param>
    public void DecreaseBossHealth(int amount)
    {
        currentHealth -= amount;
    }
    /// <summary>
    /// Handles the initialization of the boss health and status flags
    /// </summary>
    public void Activate()
    {
        currentHealth = startingHealth;
        onTheBoard = true;
    }
    /// <summary>
    /// Handles the actions to remove the boss.
    /// </summary>
    public void Deactivate()
    {
        onTheBoard = false;
    }
}
