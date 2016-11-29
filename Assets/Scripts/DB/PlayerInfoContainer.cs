using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("container")]
public class PlayerInfoContainer
{
    [XmlArray("players")] [XmlArrayItem("player")]
    public List<PlayerInfo> players = new List<PlayerInfo>();

    public string SceneIn; 

    public void Add(PlayerInfo info)
    {
        players.Add(info);
    }

    public void SaveDataToXML()
    {
        var formatter = new XmlSerializer(typeof (PlayerInfoContainer));

        string path = Application.dataPath + "/DB/person.xml";

        using (var sw = new StreamWriter(path, false, Encoding.UTF8))
        {
            formatter.Serialize(sw, this);
        }
    }

    public static PlayerInfoContainer LoadDataFromXML()
    {
        var formatter = new XmlSerializer(typeof (PlayerInfoContainer));

        try
        {
            using (var fs = new FileStream(Application.dataPath + "/DB/person.xml", FileMode.Open))
            {
                return formatter.Deserialize(fs) as PlayerInfoContainer;
            }
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}