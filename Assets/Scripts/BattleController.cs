using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    private readonly Color defaultColor = Color.black;
    private readonly FontStyle defaultStyle = FontStyle.Normal;
    private GameObject[] actions;

    private List<Action> actionsQueue = new List<Action>();

    private GameObject arrow;

    private Color choiceColor;
    private FontStyle choiceStyle;

    private BattleState currentBattleState;


    private int currentMenuChoise;
    private PanelState currentPanelState;
    private int currentPlayerTurn;
    private GameObject[] enemies;
    private GameObject[] enemyNames;

    private List<GameObject> players;
    private GameObject strike;

    public TextAsset xmlFile;

    private void Start()
    {
        LoadBattle();

        //Loading players
        LoadPlayers();

        //Loading misc and interface
        LoadInfoPanel();
        strike = GameObject.Find("strike");
        arrow = GameObject.Find("arrow");
        actions = GameObject.FindGameObjectsWithTag("Actions").OrderBy(go => go.name).ToArray();


        currentPanelState = PanelState.Actions;
        currentBattleState = BattleState.PlayerState;
        currentPlayerTurn = 0;
        currentMenuChoise = 0;

        GameObject.Find("HeroName").GetComponent<Text>().text = players[currentPlayerTurn].name;

        //Active style for text
        choiceColor = new Color(30, 190, 20, 255);
        choiceStyle = FontStyle.Bold;
    }

    private void LoadPlayers()
    {
        var playerSprites = Resources.LoadAll<Sprite>("players");
        players = new List<GameObject>();

        var spawns = GameObject.FindGameObjectsWithTag("PlayerSpawn").OrderBy(sp => sp.name).ToArray();

        for (var i = 0; i < playerSprites.Length; i++)
        {
            players.Add(new GameObject(playerSprites[i].name));
            players[i].transform.SetParent(spawns[i].transform);
            players[i].transform.position = spawns[i].transform.position;
            players[i].transform.localScale = new Vector3(1.3f, 1.3f);

            var renderer = players[i].AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 6;
            renderer.sprite = playerSprites[i];

            players[i].tag = "Player";

            var info = GameObject.FindWithTag("MainCamera").GetComponent<SaveDataContorller>().GetPlayerInfo(i);

            players[i].AddComponent<BattleCreatureInfo>().CreatePlayer(info.name,
                info.HP,
                info.attack,
                info.currentHP);

            var temp = Resources.Load<AnimationClip>("Animation/PlayerAttack");
            var anim = players[i].AddComponent<Animation>();
            anim.clip = temp;
            anim.AddClip(temp, temp.name);
            anim.playAutomatically = false;
        }

        for (var i = 0; i < players.Count; i++)
        {
            if (players[i].GetComponent<BattleCreatureInfo>().currentHP <= 0)
            {
                players.Remove(players[i]);
            }
        }
    }

    // Update is called once per frame
    private void Update()
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
            case (BattleState.EndingState):
                EndBattle();
                break;
        }
    }

    private void DoActions()
    {
        SetNewStyle(actions[currentMenuChoise].GetComponent<Text>());

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(actions[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise--;

            checkOutOfBounds(actions);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SetDefaultStyle(actions[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise++;

            checkOutOfBounds(actions);
        }

        //TEMPORARILY
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space) && actions[currentMenuChoise].name.Equals("Attack"))
        {
            var enemyPanel = GameObject.Find("EnemyChoicePanel");
            enemyPanel.GetComponent<Image>().enabled = true;
            currentPanelState = PanelState.Enemies;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyNames = new GameObject[enemies.Length];


            for (var i = 0; i < enemyNames.Length; i++)
            {
                enemyNames[i] = new GameObject(enemies[i].name);
                enemyNames[i].transform.SetParent(enemyPanel.transform);
                var text = enemyNames[i].AddComponent<Text>();
                text.text = enemies[i].name.Substring(0, enemies[i].name.Length - 1);
                text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
                text.fontSize = 12;
                text.color = Color.black;
                text.fontStyle = FontStyle.Normal;
                currentMenuChoise = 0;
            }

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

            checkOutOfBounds(enemyNames);

            arrow.transform.SetParent(enemies[currentMenuChoise].transform);

            arrow.transform.localScale = new Vector3(3, 3, 0);
            arrow.transform.position = enemies[currentMenuChoise].transform.position + new Vector3(0, 10, 0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SetDefaultStyle(enemyNames[currentMenuChoise].GetComponent<Text>());
            currentMenuChoise++;

            checkOutOfBounds(enemyNames);

            arrow.transform.SetParent(enemies[currentMenuChoise].transform.transform);

            arrow.transform.localScale = new Vector3(3, 3, 0);
            arrow.transform.position = enemies[currentMenuChoise].transform.position + new Vector3(0, 10, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            actionsQueue.Add(new Action(players[currentPlayerTurn], enemies[currentMenuChoise], Action.ActionType.Attack));

            changePlayer();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReloadGUI();
        }


        checkOutOfBounds(enemyNames);
    }

    private void checkOutOfBounds(GameObject[] array)
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
        var enemySprites = Resources.LoadAll<Sprite>("enemy");

        var enemyCounter = Random.Range(1, 5);
        enemies = new GameObject[enemyCounter];

        var xml = new XmlDocument();
        xml.LoadXml(xmlFile.text);
        var nodeList = xml.GetElementsByTagName("Enemy");

        for (var i = 0; i < enemies.Length; i++)
        {
            var numFromXml = Random.Range(0, nodeList.Count);
            enemies[i] = new GameObject(nodeList[numFromXml].Attributes["name"].Value + i);
            enemies[i].AddComponent<BattleCreatureInfo>();

            enemies[i].GetComponent<BattleCreatureInfo>().Create(enemies[i].name,
                int.Parse(nodeList[numFromXml].SelectSingleNode("HP").InnerText),
                int.Parse(nodeList[numFromXml].SelectSingleNode("Attack").InnerText));

            var renderer = enemies[i].AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 6;

            var sp = new Sprite();

            var text = nodeList[numFromXml].SelectSingleNode("Sprite").InnerText;

            foreach (var enemySprite in enemySprites)
            {
                if (enemySprite.name.Equals(text))
                {
                    sp = enemySprite;
                }
            }

            renderer.sprite = sp;

            enemies[i].transform.localScale = new Vector3(3, 3, 0);
        }

        var spawnAreas = GameObject.FindGameObjectsWithTag("Respawn").OrderBy(go => go.name).ToArray();

        for (var i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.SetParent(spawnAreas[i].transform);
            enemies[i].transform.position = spawnAreas[i].transform.position;
            enemies[i].tag = "Enemy";
        }

        var enemyHPS = new GameObject[enemies.Length];
        for (var i = 0; i < enemyHPS.Length; i++)
        {
            enemyHPS[i] = new GameObject(enemies[i].name + "hp");
            enemyHPS[i].transform.SetParent(enemies[i].transform);

            enemyHPS[i].layer = 5;
            var text = enemyHPS[i].AddComponent<TextMesh>();

            enemyHPS[i].GetComponent<MeshRenderer>().sortingOrder = 6;

            text.text = "HP " + enemies[i].GetComponent<BattleCreatureInfo>().currentHP + "/" +
                        enemies[i].GetComponent<BattleCreatureInfo>().HP;
            text.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            text.fontSize = 10;
            text.color = Color.black;
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAlignment.Left;
            enemyHPS[i].transform.position = enemies[i].transform.position - new Vector3(5, 13, 0);
        }

        foreach (var gameObject in enemies)
        {
            var temp = Resources.Load<AnimationClip>("Animation/EnemyAttack");
            var anim = gameObject.AddComponent<Animation>();
            anim.clip = temp;
            anim.AddClip(temp, temp.name);
            anim.playAutomatically = false;
        }
    }

    private void EnemyTurn()
    {
        foreach (var en in enemies)
        {
            actionsQueue.Add(new Action(en, players[Random.RandomRange(0, players.Count)], Action.ActionType.Attack));
        }

        currentBattleState = BattleState.ActionState;

        StartCoroutine(PerformTurn());
    }

    private IEnumerator PerformTurn()
    {
        ClearGUI();

        var rnd = new System.Random();
        actionsQueue = actionsQueue.OrderBy(x => rnd.Next()).ToList();

        foreach (var act in actionsQueue)
        {
            //Check if initiator or responder is already dead
            if (act.initiator == null || act.responder == null ||
                act.initiator.GetComponent<BattleCreatureInfo>().currentHP <= 0)
                continue;

            StartCoroutine(PerformAction(act));
            yield return new WaitForSeconds(2);
        }

        NextTurn();
    }

    private IEnumerator PerformAction(Action act)
    {
        var damage = 0;

        if (act.actionType == Action.ActionType.Attack)
        {
            damage = act.initiator.GetComponent<BattleCreatureInfo>().attack;

            //Animation
            act.StartAttackAnimation();
            strike.transform.position = act.responder.transform.position;
            yield return new WaitForSeconds(1);
            strike.transform.position = new Vector3(0, 0, 0);
        }

        act.responder.GetComponent<BattleCreatureInfo>().currentHP -= damage;

        if (act.responder.GetComponent<BattleCreatureInfo>().currentHP <= 0)
        {
            if (act.responder.tag == "Player")
                players.Remove(act.responder);

            else
            {
                Destroy(GameObject.Find(act.responder.name + "hp"));
                Destroy(act.responder);
            }
        }

        UpdateHP(act.responder);
    }

    private void LoadInfoPanel()
    {
        var infoPanel = GameObject.Find("InfoPanel");
        var infoPanelOriginal = GameObject.Find("InfoPanelOriginal");

        var playersPanel = new GameObject[players.Count];

        for (var i = 0; i < playersPanel.Length; i++)
        {
            playersPanel[i] = Instantiate(infoPanelOriginal, infoPanel.transform) as GameObject;
            playersPanel[i].name = players[i].name;

            playersPanel[i].GetComponent<GridLayoutGroup>().padding.left = 3;
            playersPanel[i].GetComponent<GridLayoutGroup>().padding.top = 3;
            playersPanel[i].GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 20);

            playersPanel[i].transform.SetParent(infoPanel.transform);
            playersPanel[i].GetComponent<Image>().color = Color.red;


            var textName = new GameObject("InfoNamePlayer");
            textName.transform.SetParent(playersPanel[i].transform);
            var name = textName.AddComponent<Text>();
            name.text = players[i].name;
            name.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            name.fontSize = 14;
            name.color = Color.black;
            name.fontStyle = FontStyle.Bold;

            var textHP = new GameObject("InfoHPPlayer" + players[i].name);
            textHP.transform.SetParent(playersPanel[i].transform);
            var hp = textHP.AddComponent<Text>();
            hp.text = string.Format("HP\t{0}/{1}", players[i].GetComponent<BattleCreatureInfo>().currentHP,
                players[i].GetComponent<BattleCreatureInfo>().HP);

            hp.font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            hp.fontSize = 14;
            hp.color = Color.black;
            hp.fontStyle = FontStyle.Normal;
        }
    }

    private void UpdateHP(GameObject obj)
    {
        if (obj.tag == "Player")
        {
            var text = GameObject.Find("InfoHPPlayer" + obj.name).GetComponent<Text>();
            text.text = string.Format("HP\t{0}/{1}", obj.GetComponent<BattleCreatureInfo>().currentHP,
                obj.GetComponent<BattleCreatureInfo>().HP);
        }
        else
        {
            var text = GameObject.Find(obj.name + "hp").GetComponent<TextMesh>();
            text.text = string.Format("HP {0}/{1}", obj.GetComponent<BattleCreatureInfo>().currentHP,
                obj.GetComponent<BattleCreatureInfo>().HP);
        }
    }

    private void NextTurn()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 ||
            GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            currentBattleState = BattleState.EndingState;
            return;
        }

        foreach (var act in actions)
        {
            act.GetComponent<Text>().enabled = true;
        }

        actionsQueue.Clear();
        currentBattleState = BattleState.PlayerState;
        currentPanelState = PanelState.Actions;
        currentPlayerTurn = 0;
        currentMenuChoise = 0;

        GameObject.Find("HeroName").GetComponent<Text>().text = players[currentPlayerTurn].name;
    }

    private void ClearGUI()
    {
        GameObject.Find("HeroName").GetComponent<Text>().text = "";
        arrow.transform.position = new Vector2(0, 0);
        arrow.transform.parent = null;
        GameObject.Find("EnemyChoicePanel").GetComponent<Image>().enabled = false;

        foreach (var obj in enemyNames)
        {
            Destroy(obj);
        }

        foreach (var act in actions)
        {
            act.GetComponent<Text>().enabled = false;
        }
    }

    private void ReloadGUI()
    {
        arrow.transform.position = new Vector2(0, 0);
        GameObject.Find("EnemyChoicePanel").GetComponent<Image>().enabled = false;
        foreach (var obj in enemyNames)
        {
            Destroy(obj);
        }
        currentPanelState = PanelState.Actions;
        currentMenuChoise = 0;
    }


    private void EndBattle()
    {
        if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            Application.Quit();
        }
        else
        {
            players = GameObject.FindGameObjectsWithTag("Player").ToList();
            var pinfo = new List<PlayerInfo>(players.Count);
            pinfo.AddRange(players.Select(t => new PlayerInfo(t.GetComponent<BattleCreatureInfo>().name,
                t.GetComponent<BattleCreatureInfo>().HP,
                t.GetComponent<BattleCreatureInfo>().attack,
                t.GetComponent<BattleCreatureInfo>().currentHP)));

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SaveDataContorller>().ChangePlayerInfo(pinfo);
            SceneManager.LoadScene("forest");
        }
    }

    private class Action
    {
        public enum ActionType
        {
            Attack,
            Magic
        }

        public Action(GameObject initiator, GameObject responder, ActionType actionType)
        {
            this.initiator = initiator;
            this.responder = responder;
            this.actionType = actionType;
        }

        public GameObject initiator { get; set; }
        public GameObject responder { get; set; }
        public ActionType actionType { get; set; }

        public void StartAttackAnimation()
        {
            //initiator.transform.position = initiator.transform.localPosition;
            initiator.GetComponent<Animation>().Play();
        }

        public override string ToString()
        {
            return (initiator.name + "\t" + responder.name + "\t" + actionType);
        }
    }

    private enum BattleState
    {
        PlayerState,
        EnemyState,
        ActionState,
        EndingState
    };

    private enum PanelState
    {
        Actions,
        Enemies
    };

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
        if (currentPlayerTurn == players.Count - 1)
        {
            currentBattleState = BattleState.EnemyState;
            return;
        }
        currentPlayerTurn++;
        GameObject.Find("HeroName").GetComponent<Text>().text = players[currentPlayerTurn].name;
        currentPanelState = PanelState.Actions;
        currentMenuChoise = 0;
        foreach (var act in actions)
        {
            act.GetComponent<Text>().enabled = true;
        }
    }

    #endregion
}