using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{

    enum PanelState
    {
        MainMenu,
        Saves
    }

    private Color defaultColor = Color.black;
    private FontStyle defaultStyle = FontStyle.Normal;

    public Color choiceColor;
    public FontStyle choiceStyle;

    private GameObject[] mainActions;
    private GameObject[] saveNames;

    private int currentChoice;
    private PanelState panelState;
    private GameObject savePanel;

    private GameObject[] saves;

    // Use this for initialization
    void Start()
    {
        mainActions = GameObject.FindGameObjectsWithTag("Actions").OrderBy(act => act.name).ToArray();
        savePanel = GameObject.Find("SavePanel");

        currentChoice = 0;
        panelState = PanelState.MainMenu;

        choiceColor = new Color(30, 190, 20, 255);
        choiceStyle = FontStyle.Bold;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    switch (panelState)
	    {
            case (PanelState.MainMenu):
	            ChooseMenu();
	            break;
            case (PanelState.Saves):
	            ChooseSave();
	            break;
	    }
	}

    private void ChooseMenu()
    {
        SetNewStyle(mainActions[currentChoice].GetComponent<Text>());
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDefaultStyle(mainActions[currentChoice].GetComponent<Text>());
            currentChoice++;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(mainActions[currentChoice].GetComponent<Text>());
            currentChoice--;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (mainActions[currentChoice].GetComponent<Text>().text)
            {
                case ("Load"):
                    LoadSaves();
                    panelState = PanelState.Saves;
                    break;

                default:
                    break;
            }
        }

        CheckOutOFBounds(mainActions);
    }

    private void LoadSaves()
    {
        savePanel.GetComponent<Image>().enabled = true;

        var saveNames = new DirectoryInfo(Application.dataPath + "/Saves").GetFiles("*.xml").OrderBy(s => s.Name).ToArray();

        saves = new GameObject[saveNames.Length];

        for (int i = 0; i < saveNames.Length; i++)
        {
            saves[i] = new GameObject(saveNames[i].Name);
            var text = saves[i].AddComponent<Text>();
            text.text = saveNames[i].Name.Substring(0, saveNames[i].Name.Length-4);
            saves[i].transform.SetParent(GameObject.Find("SavePanel").transform);
            text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.fontSize = 20;
            text.color = Color.black;
            text.fontStyle = FontStyle.Normal;
        }
        currentChoice = 0;
    }

    private void CheckOutOFBounds(GameObject[] gameObjects)
    {
        if (currentChoice > gameObjects.Length - 1)
            currentChoice = 0;
        else if (currentChoice < 0)
            currentChoice = gameObjects.Length - 1;
    }

    private void SetDefaultStyle(Text t)
    {
        t.fontStyle = defaultStyle;
        t.color = defaultColor;
    }

    private void SetNewStyle(Text t)
    {
        t.fontStyle = choiceStyle;
        t.color = choiceColor;
    }

    private void ChooseSave()
    {
        SetNewStyle(saves[currentChoice].GetComponent<Text>());

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDefaultStyle(saves[currentChoice].GetComponent<Text>());
            currentChoice--;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(saves[currentChoice].GetComponent<Text>());
            currentChoice++;
        }

        CheckOutOFBounds(saves);
    }
}
