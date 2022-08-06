using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEncounter : EncounterCard
{
    [SerializeField] private List<int> bigWinCriteria;
    [SerializeField] private Action bigWinResult;
    [SerializeField, Range(1, 2)] private int bigWinResultAmount;

    public bool IsResultBigWin(int rollValue)
    {
        return bigWinCriteria.Contains(rollValue);
    }

    public string GetBigWinAction()
    {
        return bigWinResult.ToString();
    }

    public int GetResultAmount(bool isWin, bool isBigWin)
    {
        if(isWin)
        {
            return ResultAmount(true);
        }
        else if (isBigWin)
        {
            return bigWinResultAmount;
        }
        else
        {
            return ResultAmount(false);
        }
    }
}
