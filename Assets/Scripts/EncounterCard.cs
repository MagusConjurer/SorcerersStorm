using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterCard : Card
{
    [SerializeField] private Type EncounterType;
    [SerializeField, Range(1, 6)] private int enemyStrength;
    [SerializeField] private List<int> winCriteria = new List<int> {7,8,9,10,11,12};
    [SerializeField] private List<int> loseCriteria = new List<int> {1,2,3,4,5,6};

    

    [SerializeField] private Action WinResult;
    [SerializeField, Range(1, 2)] private int WinResultAmount;
    [SerializeField] private Action LossResult;
    [SerializeField, Range(1, 2)] private int LossResultAmount;

    public string GetEncounterType()
    {
        return EncounterType.ToString();
    }

    public bool IsResultWin(int rollValue)
    {
        return winCriteria.Contains(rollValue);
    }

    public string ResultAction(bool isWin)
    {
        if(isWin)
        {
            return WinResult.ToString();
        }
        else
        {
            return LossResult.ToString();
        }
    }

    public int ResultAmount(bool isWin)
    {
        if (isWin)
        {
            return WinResultAmount;
        }
        else
        {
            return LossResultAmount;
        }
    }

    private enum Action
    {
        Item,
        Health,
        Strength
    }

    private enum Type 
    {
        Enemy, 
        Trap, 
        SkillCheck, 
        Unlockable
    }
}
