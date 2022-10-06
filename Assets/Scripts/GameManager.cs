using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static private GameManager _instance;
    private Boss[] bosses;
    private int level;

    private GameManager()
    {
        level = 1;
        bosses[0] = new Boss("The Sorcerer", 5);
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
        return bosses[level - 1];
    }

    public int GetLevel()
    {
        return level;
    }
}
