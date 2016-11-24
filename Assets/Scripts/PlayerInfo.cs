using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class PlayerInfo
{
    [XmlAttribute("name")]
    public string name;
    public int HP;
    public int attack;

    public int currentHP;

    public PlayerInfo()
    {

    }

    public PlayerInfo(string name, int HP, int attack)
    {
        this.name = name;
        this.HP = HP;
        this.attack = attack;
        currentHP = HP;
    }

    
}
