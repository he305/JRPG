using System.Xml.Serialization;
public class PlayerInfo
{
    public int attack;

    public int currentHP;
    public int HP;

    [XmlAttribute("name")]
    public string name;

    public PlayerInfo()
    {
    }

    public PlayerInfo(string name, int HP, int attack, int currentHP)
    {
        this.name = name;
        this.HP = HP;
        this.attack = attack;
        this.currentHP = currentHP;
    }
}
