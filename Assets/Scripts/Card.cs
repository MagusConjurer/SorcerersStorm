using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Card : MonoBehaviour
{
    [SerializeField] protected CardManager cardManager;
    [SerializeField] protected bool selected;
    protected SpriteRenderer cardSprite;

    private SpriteRenderer border;

    protected virtual void Start()
    {
        try
        {
            cardManager = FindObjectOfType<GameManager>().GetComponent<CardManager>();
            cardSprite = GetComponent<SpriteRenderer>();

            border = transform.Find("Border").GetComponent<SpriteRenderer>();
            border.enabled = false;
        }
        catch
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    /// <summary>
    /// Makes the card inactive (not visible)
    /// </summary>
    protected void HideCard()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// Used to update to a selected state
    /// </summary>
    public void SelectCard()
    {
        selected = true;
        border.enabled = true;
    }
    /// <summary>
    /// Used to update to an unselected state
    /// </summary>
    public void UnselectCard()
    {
        selected = false;
        border.enabled = false;
    }

}
