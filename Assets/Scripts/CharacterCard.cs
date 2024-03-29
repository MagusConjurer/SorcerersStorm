using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card
{
    [SerializeField] private int baseHealth, baseStrength, baseAccuracy, baseStealth;
    [SerializeField] private int currentHealth, currentStrength, currentAccuracy, currentStealth;
    [SerializeField] private List<string> possibleTypes = new List<string>{ "Health", "Strength", "Accuracy", "Stealth" };
    [SerializeField] private Color inactiveStatColor, activeStatColor;

    public int placedIndex;
    public bool isAlive;
    public bool inTeam;
    public bool confirmedInTeam;

    protected override void Start()
    {
        base.Start();

        SetHealth(baseHealth);
        SetStrength(baseStrength);
        SetAccuracy(baseAccuracy);
        SetStealth(baseStealth);
        isAlive = true;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetStrength()
    {
        return currentStrength;
    }

    public int GetAccuracy()
    {
        return currentAccuracy;
    }

    public int GetStealth()
    {
        return currentStealth;
    }

    public void DecreaseHealth(int value)
    {
        SetHealth(currentHealth - value);

        if (currentHealth <= 0)
        {
            isAlive = false;
            HideCard();
            cardManager.RemoveFromTeam(this);
        }
    }

    public void DecreaseStrength(int value)
    {
        SetStrength(currentStrength - value);
    }

    public void DecreaseAccuracy(int value)
    {
        SetAccuracy(currentAccuracy - value);
    }

    public void DecreaseStealth(int value)
    {
        SetStealth(currentStealth - value);
    }

    public void IncreaseHealth(int value)
    {
        SetHealth(currentHealth + value);
    }

    public void IncreaseStrength(int value)
    {
        SetStrength(currentStrength + value);
    }

    public void IncreaseAccuracy(int value)
    {
        SetAccuracy(currentAccuracy + value);
    }

    public void IncreaseStealth(int value)
    {
        SetStealth(currentStealth + value);
    }

    private void SetHealth(int value)
    {
        currentHealth = value;
        if (currentHealth < 0) currentHealth = 0;

        if (currentHealth >= 0 && value >= 0)
        {
            SpriteRenderer[] healthSprites = GetStatSprites("Health");
            SetStatSprites(healthSprites, value);
        }
    }

    private void SetStrength(int value)
    {
        currentStrength = value;
        if (currentStrength < 0) currentStrength = 0;

        if (currentStrength >= 0 && value >= 0)
        {
            SpriteRenderer[] strengthSprites = GetStatSprites("Strength");
            SetStatSprites(strengthSprites, value);
        }
    }

    private void SetAccuracy(int value)
    {
        currentAccuracy = value;
        if(currentAccuracy < 0) currentAccuracy = 0;

        if (currentAccuracy >= 0 && value >= 0)
        {
            SpriteRenderer[] accuracySprites = GetStatSprites("Accuracy");    
            SetStatSprites(accuracySprites, value);
        }
    }

    private void SetStealth(int value)
    {
        currentStealth = value;
        if (currentStealth < 0) currentStealth = 0;

        if (currentStealth >= 0 && value >= 0)
        {
            SpriteRenderer[] stealthSprites = GetStatSprites("Stealth");
            SetStatSprites(stealthSprites, value);
        }
    }

    private void OnMouseDown()
    {
        if (selected == false)
        {
            if(inTeam == false)
            {
                cardManager.AddToTeam(this);
            }
            else
            {
                cardManager.SetCurrentCharacter(this);
            }
        }
        else
        {
            if(inTeam == true && confirmedInTeam == false)
            {
                cardManager.RemoveFromTeam(this);
            }
            else
            {
                cardManager.UnsetCurrentCharacter(this);
            }
        }
    }

    /// <summary>
    /// Helper method that allows the SpriteRenderers for the stat squares to be returned.
    /// </summary>
    /// <param name="type">The name of the child element that contains the SpriteRenderers</param>
    /// <returns>An array of SpriteRenderers, length zero if the type was invalid</returns>
    private SpriteRenderer[] GetStatSprites(string type)
    {
        if(possibleTypes.Contains(type))
        {
            foreach (Transform child in transform)
            {
                if (child.name == type)
                {
                    return child.GetComponentsInChildren<SpriteRenderer>();
                }
            }
        }

        Debug.Log("Type given to GetStatSprites was not found.");
        return new SpriteRenderer[0];
    }

    /// <summary>
    /// Changes the color of the sprites to indicate the current stat value.
    /// </summary>
    /// <param name="sprites">Array of sprites to be updated</param>
    /// <param name="value">Number of sprites to set active</param>
    private void SetStatSprites(SpriteRenderer[] sprites, int value)
    {
        if(sprites.Length > 0)
        {
            for(int i = 0; i < value; i++)
            {
                sprites[i].color = activeStatColor;
            }
            for(int j = value; j < sprites.Length; j++)
            {
                sprites[j].color = inactiveStatColor;
            }
        }
        else
        {
            Debug.Log("No sprites available to set the stat for.");
        }
    }
}
