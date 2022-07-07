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

    protected override void Start()
    {
        base.Start();
        SetHealth(baseHealth);
        SetStrength(baseStrength);
        SetAccuracy(baseAccuracy);
        SetStealth(baseStealth);
        isAlive = true;
    }

    private void Update()
    {
        if(currentHealth <= 0)
        {
            isAlive = false;
            HandleDeath();
        }
    }

    public void ResetColor()
    {
        cardSprite.color = cardSprite.color * 2;
    }

    public void SetHealth(int value)
    {
        SpriteRenderer[] healthSprites = GetStatSprites("Health");
        currentHealth = value;
        SetStatSprites(healthSprites, value);
    }

    public void SetStrength(int value)
    {
        SpriteRenderer[] strengthSprites = GetStatSprites("Strength");
        currentStrength = value;
        SetStatSprites(strengthSprites, value);
    }

    public void SetAccuracy(int value)
    {
        SpriteRenderer[] accuracySprites = GetStatSprites("Accuracy");
        currentAccuracy = value;
        SetStatSprites(accuracySprites, value);
    }

    public void SetStealth(int value)
    {
        SpriteRenderer[] stealthSprites = GetStatSprites("Stealth");
        currentStealth = value;
        SetStatSprites(stealthSprites, value);
    }


    private void OnMouseDown()
    {
        if (selected == false)
        {
            cardManager.SelectCharacter(this);
            selected = true;
            cardSprite.color = cardSprite.color / 2;
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

    /// <summary>
    /// 
    /// </summary>
    private void HandleDeath()
    {
        // TODO: Implement updating the character when their health is 0.
        // most likely will want parent method for this
    }
}
