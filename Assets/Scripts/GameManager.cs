using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;
    private List<Boss> bosses;
    private int currentLevel;
    private bool gameStarted;

    private GameManager()
    {
        gameStarted = false;
    }

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("No GameManager!");

            }
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public Boss GetCurrentBoss()
    {
        return bosses[currentLevel - 1];
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public void StartGame()
    {
        currentLevel = 1;
        bosses = new List<Boss>();
        bosses.Add(new Boss("The Sorcerer", 5));
        gameStarted = true;
    }

    public void EndGame()
    {
        gameStarted = false;
    }
}
