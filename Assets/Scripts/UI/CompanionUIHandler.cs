using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionUIHandler : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] CharacterUI;
    public String[] name;
    public String[] description;
    public String[] age;
    public String[] race;

    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Age;
    public TextMeshProUGUI Race;
    private int activatedIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        assignButtons();
    }
    
    
    void updateCharacter(int index)
    {
       CharacterUI[activatedIndex].SetActive(false);
       CharacterUI[index].SetActive(true);
       Name.text = name[index];
       Description.text = description[index];
       Age.text = age[index];
       Race.text = race[index];
        activatedIndex = index;
    }

    void assignButtons()
    {
        buttons[0].onClick.AddListener(()=> updateCharacter(0));
        buttons[1].onClick.AddListener(()=> updateCharacter(1));
        buttons[2].onClick.AddListener(()=> updateCharacter(2));
        buttons[3].onClick.AddListener(()=> updateCharacter(3));
        buttons[4].onClick.AddListener(()=> updateCharacter(4));
        buttons[5].onClick.AddListener(()=> updateCharacter(5));
        buttons[6].onClick.AddListener(()=> updateCharacter(6));
    }
}
