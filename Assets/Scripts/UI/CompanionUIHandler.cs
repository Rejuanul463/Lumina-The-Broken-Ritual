using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Companion information screen. Shows a 3D/2D preview object and text details (name, description,
/// age, race) for one companion at a time. Each button in "buttons" selects the companion with the
/// same index. All the arrays must have the same length (7 entries are expected, see assignButtons).
/// </summary>
public class CompanionUIHandler : MonoBehaviour
{
    // Selection buttons, one per companion
    public Button[] buttons;
    // Preview objects, one per companion (only the selected one is active)
    public GameObject[] CharacterUI;
    // Text data per companion (same index order as buttons/CharacterUI)
    // NOTE: "name" hides the inherited Object.name member; it works but produces a compiler warning.
    public String[] name;
    public String[] description;
    public String[] age;
    public String[] race;

    // Text fields on the UI that display the selected companion's details
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Age;
    public TextMeshProUGUI Race;
    // Index of the currently shown companion
    private int activatedIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        assignButtons();
    }


    // Hides the previous companion's preview, shows the new one and fills in the texts
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

    // Connects button N to companion N. Hard-coded for 7 buttons; add a line here if more are added.
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
