using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss
{
    private int startingHealth = 5;
    private bool onTheBoard;
    private int currentHealth;
    private string bossName;
    public Boss(string name)
    {
        onTheBoard = false;
        bossName = name;
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
