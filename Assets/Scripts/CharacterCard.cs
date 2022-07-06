using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCard : Card
{
    [SerializeField] private int healthValue;
    [SerializeField] private int strengthValue;
    [SerializeField] private int accuracyValue;
    [SerializeField] private int stealthValue;

    public int placedIndex;
    

    private void OnMouseDown()
    {
        if (selected == false)
        {
            cardManager.SelectCharacter(this);
            selected = true;
            cardSprite.color = cardSprite.color / 2;
        }
    }

    public void ResetColor()
    {
        cardSprite.color = cardSprite.color * 2;
    }

    public void SetHealth(int value)
    {
        SpriteRenderer[] healthSprites = GetStatSprites("Health");
        SetStatSprites(healthSprites, value);
    }

    public void SetStrength(int value)
    {
        SpriteRenderer[] strengthSprites = GetStatSprites("Strength");
        SetStatSprites(strengthSprites, value);
    }

    public void SetAccuracy(int value)
    {
        SpriteRenderer[] accuracySprites = GetStatSprites("Accuracy");
        SetStatSprites(accuracySprites, value);
    }

    public void SetStealth(int value)
    {
        SpriteRenderer[] stealthSprites = GetStatSprites("Stealth");
        SetStatSprites(stealthSprites, value);
    }

    private SpriteRenderer[] GetStatSprites(string type)
    {
        // TODO: Implement getting the sprite squares
        return new SpriteRenderer[1];
    }

    private void SetStatSprites(SpriteRenderer[] sprites, int value)
    {
        // TODO: Implement setting the sprites
    }
}
