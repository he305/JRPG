using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections;

public class BattleController : MonoBehaviour {

    class Action
    {
        public enum ActionType
        {
            Attack,
            Magic
        }

        public GameObject initiator { get; set; }
        public GameObject responder { get; set; }
        public ActionType actionType { get; set; }

        public Action(GameObject initiator, GameObject responder, ActionType actionType)
        {
            this.initiator = initiator;
            this.responder = responder;
            this.actionType = actionType;
        }

        public override string ToString()
        {
            return (initiator.name + "\t" + responder.name + "\t" + actionType);
        }
    }

    enum BattleState
    {
        PlayerState,
        EnemyState,
        ActionState
    };

    enum PanelState
    {
        Actions,
        Enemies
    };

    //Battle state
    BattleState currentBattleState;

    //Actions list
    private List<Action> actionsQueue = new List<Action>();

    //XMLFile
    public TextAsset xmlFile;

    //Arrow
    public Sprite arrowSprite;
    GameObject arrow;


    //First action panel
    private int currentMenuChoise;
    private GameObject[] actions;
    private PanelState currentPanelState;

    //Player + Enemy panel
    private GameObject[] players;
    private int currentPlayerTurn;
    private GameObject[] enemies;
    private GameObject[] enemyNames;


    //Data for texts;
    private Color defaultColor = Color.black;
    private FontStyle defaultStyle = FontStyle.Normal;

    private Color choiceColor;
    private FontStyle choiceStyle;

    // Use this for initialization
    void Start() {

        currentBattleState = BattleState.PlayerState;
        players = GameObject.FindGameObjectsWithTag("Player").OrderBy(p => p.name).ToArray();

        //PLAYER CREATION
        //
        //
        foreach(GameObject player in players)
        {
            player.AddComponent<BattleCreatureInfo>().Create(player.name, 100, 20);
        }
        //TEMPORARILY USAGE

        LoadInfoPanel();
        currentPlayerTurn = 0;
        GameObject.Find("HeroName").GetComponent<Text>().text = players[currentPlayerTurn].name;
        LoadBattle();

        currentPanelState = PanelState.Actions;


        actions = GameObject.FindGameObjectsWithTag("Actions").OrderBy(go => go.name).ToArray();

        currentMenuChoise = 0;

        choiceColor = new Color(30, 190, 20, 255);
        choiceStyle = FontStyle.Bold;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentBattleState)
        {
            case (BattleState.PlayerState):
                switch (currentPanelState)
                {
                    case (PanelState.Actions):
                        DoActions();
                        break;
                    case (PanelState.Enemies):
                        ChooseEnemy();
                        break;
                }
                break;
            case (BattleState.EnemyState):
                EnemyTurn();
                break;
            case (BattleState.ActionState):
                break;
        }
    }

    private void DoActions()
    {
        GameObject.Find("HeroName").GetComponent<Text>().text = players[currentPlayerTurn].name;
        SetNewStyle(actions[currentMenuChoise].GetComponent<Text>());

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(actions[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise--;

            checkOutOfBounds(currentMenuChoise, actions);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDefaultStyle(actions[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise++;

            checkOutOfBounds(currentMenuChoise, actions);
        }

        if (Input.GetKeyDown(KeyCode.Space) && actions[currentMenuChoise].name.Equals("Attack"))
        {
            GameObject enemyPanel = GameObject.Find("EnemyChoicePanel");
            enemyPanel.GetComponent<Image>().enabled = true;
            currentPanelState = PanelState.Enemies;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyNames = new GameObject[enemies.Length];



            for (int i = 0; i < enemyNames.Length; i++)
            {

                enemyNames[i] = new GameObject(enemies[i].name);
                enemyNames[i].transform.SetParent(enemyPanel.transform);
                Text text = enemyNames[i].AddComponent<Text>();
                text.text = enemies[i].name;
                text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                text.fontSize = 12;
                text.color = Color.black;
                text.fontStyle = FontStyle.Normal;
                currentMenuChoise = 0;

            }


            //Create arrow
            arrow = new GameObject("Arrow");
            SpriteRenderer renderer = arrow.AddComponent<SpriteRenderer>();
            renderer.sprite = arrowSprite;
            renderer.sortingOrder = 10;
            arrow.transform.localScale = new Vector3(3, 3, 0);


            arrow.transform.SetParent(enemies[currentMenuChoise].transform);
            arrow.transform.position = enemies[currentMenuChoise].transform.position + new Vector3(0, 10, 0);
        }
    }

    private void ChooseEnemy()
    {
        SetNewStyle(enemyNames[currentMenuChoise].GetComponent<Text>());

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDefaultStyle(enemyNames[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise--;

            checkOutOfBounds(currentMenuChoise, enemyNames);

            arrow.transform.SetParent(enemies[currentMenuChoise].transform);

            arrow.transform.localScale = new Vector3(3, 3, 0);
            arrow.transform.position = enemies[currentMenuChoise].transform.position + new Vector3(0, 10, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(enemyNames[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise++;

            checkOutOfBounds(currentMenuChoise, enemyNames);

            arrow.transform.SetParent(enemies[currentMenuChoise].transform.transform);

            arrow.transform.localScale = new Vector3(3, 3, 0);
            arrow.transform.position = enemies[currentMenuChoise].transform.position + new Vector3(0, 10, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            actionsQueue.Add(new Action(players[currentPlayerTurn], enemies[currentMenuChoise], Action.ActionType.Attack));

            changePlayer();
        }


        checkOutOfBounds(currentMenuChoise, enemyNames);
    }

    private void checkOutOfBounds(int choice, GameObject[] array)
    {
        if (currentMenuChoise < 0)
        {
            currentMenuChoise = array.Length - 1;
        }
        else if (currentMenuChoise > array.Length - 1)
        {
            currentMenuChoise = 0;
        }
    }

    private void LoadBattle()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("enemy");

        int enemyCounter = UnityEngine.Random.Range(1, 5);
        enemies = new GameObject[enemyCounter];

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(xmlFile.text);
        XmlNodeList nodeList = xml.GetElementsByTagName("Enemy");

        for (int i = 0; i < enemies.Length; i++)
        {
            int numFromXML = UnityEngine.Random.Range(0, nodeList.Count);
            enemies[i] = new GameObject(nodeList[numFromXML].Attributes["name"].Value + i);
            enemies[i].AddComponent<BattleCreatureInfo>();

            enemies[i].GetComponent<BattleCreatureInfo>().Create(enemies[i].name,
                                                                 int.Parse(nodeList[numFromXML].SelectSingleNode("HP").InnerText),
                                                                 int.Parse(nodeList[numFromXML].SelectSingleNode("Attack").InnerText));

            SpriteRenderer renderer = enemies[i].AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 6;

            Sprite sp = new Sprite();

            string text = nodeList[numFromXML].SelectSingleNode("Sprite").InnerText;

            for (int j = 0; j < sprites.Length; j++)
            {
                if (sprites[j].name.Equals(text))
                {
                    sp = sprites[j];
                }
            }

            renderer.sprite = sp;

            enemies[i].transform.localScale = new Vector3(3, 3, 0);

        }

        GameObject[] spawnAreas = GameObject.FindGameObjectsWithTag("Respawn").OrderBy(go => go.name).ToArray();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = spawnAreas[i].transform.position;
            enemies[i].tag = "Enemy";
        }

        GameObject[] enemyHPS = new GameObject[enemies.Length];
        for(int i = 0; i < enemyHPS.Length; i++)
        {
            enemyHPS[i] = new GameObject(enemies[i].name + "hp");

            enemyHPS[i].layer = 5;
            TextMesh text = enemyHPS[i].AddComponent<TextMesh>();

            enemyHPS[i].GetComponent<MeshRenderer>().sortingOrder = 6;
            
            text.text = "HP " + enemies[i].GetComponent<BattleCreatureInfo>().currentHP + "/" + enemies[i].GetComponent<BattleCreatureInfo>().HP;
            text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.fontSize = 30;
            text.color = Color.black;
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAlignment.Left;
            enemyHPS[i].transform.position = enemies[i].transform.position - new Vector3(5, 13, 0);
        }
    }

    private void EnemyTurn()
    {
        foreach(GameObject en in enemies)
        {
            actionsQueue.Add(new Action(en, players[UnityEngine.Random.RandomRange(0, players.Length)], Action.ActionType.Attack));
        }

        currentBattleState = BattleState.ActionState;

        StartCoroutine(PerformTurn());
    }

    IEnumerator PerformTurn()
    {
        System.Random rnd = new System.Random();
        actionsQueue = actionsQueue.OrderBy(x => rnd.Next()).ToList();

        foreach (Action act in actionsQueue)
        {
            PerformAction(act);
            yield return new WaitForSeconds(1);
        }

        NextTurn();
    }

    void PerformAction(Action act)
    {
        //Check if initiator is already dead
        if (act.initiator == null)
            return;
        
        int damage = 0;
        if (act.actionType == Action.ActionType.Attack)
        {
            damage = act.initiator.GetComponent<BattleCreatureInfo>().attack;
        }

        act.responder.GetComponent<BattleCreatureInfo>().currentHP -= damage;

        if (act.responder.GetComponent<BattleCreatureInfo>().currentHP <= 0)
        {
            Debug.Log(act.responder.name + "hp");
            Destroy(GameObject.Find(act.responder.name + "hp"));
            Destroy(act.responder);
        }
        UpdateHP(act.responder);
    }

    private void LoadInfoPanel()
    {
        GameObject infoPanel = GameObject.Find("InfoPanel");

        GameObject[] playersPanel = new GameObject[players.Length];

        for (int i = 0; i < playersPanel.Length; i++)
        {
            playersPanel[i] = Instantiate(infoPanel, infoPanel.transform) as GameObject;
            playersPanel[i].name = players[i].name;

            playersPanel[i].GetComponent<GridLayoutGroup>().padding.left = 3;
            playersPanel[i].GetComponent<GridLayoutGroup>().padding.top = 3;
            playersPanel[i].GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 15);

            playersPanel[i].transform.SetParent(infoPanel.transform);
            playersPanel[i].GetComponent<Image>().color = Color.red;


            GameObject textName = new GameObject("InfoNamePlayer");
            textName.transform.SetParent(playersPanel[i].transform);
            Text name = textName.AddComponent<Text>();
            name.text = players[i].name;
            name.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            name.fontSize = 10;
            name.color = Color.black;
            name.fontStyle = FontStyle.Normal;

            GameObject textHP = new GameObject("InfoHPPlayer" + players[i].name);
            textHP.transform.SetParent(playersPanel[i].transform);
            Text hp = textHP.AddComponent<Text>();
            hp.text = ("HP\t" + players[i].GetComponent<BattleCreatureInfo>().currentHP + "/" + players[i].GetComponent<BattleCreatureInfo>().HP);

            hp.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            hp.fontSize = 10;
            hp.color = Color.black;
            hp.fontStyle = FontStyle.Normal;
        }
    }

    private void UpdateHP(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            Text text = GameObject.Find("InfoHPPlayer" + obj.name).GetComponent<Text>();
            text.text = "HP\t" + obj.GetComponent<BattleCreatureInfo>().currentHP + "/" + obj.GetComponent<BattleCreatureInfo>().HP;
        }
        else
        {
            TextMesh text = GameObject.Find(obj.name + "hp").GetComponent<TextMesh>();
            text.text = "HP " + obj.GetComponent<BattleCreatureInfo>().currentHP + "/" + obj.GetComponent<BattleCreatureInfo>().HP;
        }
    }

    private void NextTurn()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            Application.Quit();
        }

        foreach (GameObject act in actions)
        {
            act.GetComponent<Text>().enabled = true;
        }

        actionsQueue.Clear();
        currentBattleState = BattleState.PlayerState;
        currentPanelState = PanelState.Actions;
        currentPlayerTurn = 0;
        currentMenuChoise = 0;

    }

    private void ClearGUI()
    {
        currentMenuChoise = 10;
        Destroy(arrow);
        GameObject.Find("EnemyChoicePanel").GetComponent<Image>().enabled = false;
        
        foreach (GameObject obj in enemyNames)
        {
            Destroy(obj);
        }

        foreach(GameObject act in actions)
        {
            act.GetComponent<Text>().enabled = false;
        }
    }

    #region Служебные методы
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

    private void changePlayer()
    {
        ClearGUI();
        if (currentPlayerTurn == players.Length - 1)
        {
            currentBattleState = BattleState.EnemyState;
        }
        currentPlayerTurn++;
    }
    #endregion
}
