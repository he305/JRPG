using UnityEngine;
using System.Collections;

public class BattleCreatureInfo : MonoBehaviour {

    public string name;
    public int HP;
    public int attack;

    public int currentHP;

    public void Create(string name, int HP, int attack)
    {
        this.name = name;
        this.HP = HP;
        this.attack = attack;
        currentHP = HP;
    }
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override string ToString()
    {
        return (name + "\t" + HP + "\t" + attack);
    }
}
