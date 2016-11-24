using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts
{
    [XmlRoot("container")]
    public class PlayerInfoContainer
    {
        [XmlArray("players")]
        [XmlArrayItem("player")]
        public List<PlayerInfo> players = new List<PlayerInfo>();

        public void Add(PlayerInfo info)
        {
            players.Add(info);
        }

        public void SaveDataToXML()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(PlayerInfoContainer));

            using (FileStream fs = new FileStream(Application.dataPath + "/DB/person.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, this);
            }
        }
    }
}
