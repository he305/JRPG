using UnityEngine;

public class BattleCreatureInfo : MonoBehaviour
{
    public int attack;

    public int currentHP;
    public int HP;

    public string name;

    public void Create(string name, int HP, int attack)
    {
        this.name = name;
        this.HP = HP;
        this.attack = attack;
        currentHP = HP;
    }

    public void CreatePlayer(string name, int HP, int attack, int currentHP)
    {
        this.name = name;
        this.HP = HP;
        this.attack = attack;
        this.currentHP = currentHP;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (currentHP < 0)
            currentHP = 0;
    }

    public override string ToString()
    {
        return (name + "\t" + HP + "\t" + attack);
    }
}