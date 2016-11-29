using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoController : MonoBehaviour
{
    private PlayerInfoContainer container;

    public void LoadController()
    {

        if (PlayerInfoContainer.LoadDataFromXML() != null)
        {
            container = PlayerInfoContainer.LoadDataFromXML();
            return;
        }
        container = new PlayerInfoContainer();
        container.Add(new PlayerInfo("Billy", 100, 20, 100));
        container.Add(new PlayerInfo("Yarik", 100, 30, 100));
        container.SaveDataToXML();
    }

    public PlayerInfo GetInfo(int i)
    {
        return container.players[i];
    }

    public List<PlayerInfo> GetAllPlayersInfo()
    {
        return container.players;
    }

    public void LoadDataToXML(List<PlayerInfo> info)
    {
        container.players = info;
        container.SaveDataToXML();
    }
}