using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TeamNPC currentCompanion;
    
    public GameObject companionUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AssignHero(TeamNPC hero)
    {
        currentCompanion = hero;
    }
    
}
