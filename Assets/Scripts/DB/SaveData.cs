using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;


[XmlRoot("save")]
public class SaveData
{
    public List<PlayerInfo> playerInfoContainer;
    public List<SceneInfo> sceneContainer;

    public string currentScene;

    public SaveData()
    {
    
    }

    public SaveData(List<PlayerInfo> playerInfoContainer, List<SceneInfo> sceneContainer, string currentScene)
    {
        this.playerInfoContainer = playerInfoContainer;
        this.sceneContainer = sceneContainer;
        this.currentScene = currentScene;
    }

    public List<PlayerInfo> GetPlayerInfoContainer()
    {
        return playerInfoContainer;
    }

    public SceneInfo GetSceneByName(string sceneName)
    {
        return sceneContainer.FirstOrDefault(sceneInfo => sceneInfo.name == sceneName);
    }

    public PlayerInfo GetPlayerInfo(int i)
    {
        return playerInfoContainer[i];
    }

    public void SaveDataToXML(string saveName)
    {
        var formatter = new XmlSerializer(typeof(SaveData));

        string path = Application.dataPath + "/Saves/" + saveName + ".xml";

        using (var sw = new StreamWriter(path, false, Encoding.UTF8))
        {
            formatter.Serialize(sw, this);
        }
    }

    public static SaveData LoadDataFromXML(string saveName)
    {
        var formatter = new XmlSerializer(typeof(SaveData));

        try
        {
            string path = Application.dataPath + "/Saves/" + saveName + ".xml";
            using (var fs = new FileStream(path, FileMode.Open))
            {
                return formatter.Deserialize(fs) as SaveData;
            }
        }
        catch (Exception)
        {
            return null;
        }
    }
}

