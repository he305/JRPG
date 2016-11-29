using UnityEngine;
using System.Xml.Serialization;

public class SceneInfo
{
    [XmlAttribute("name")]
    public string name { get; set; }
    public Vector3 playerPosition { get; set; }

    public SceneInfo()
    {
        
    }

    public SceneInfo(string name, Vector3 playerPosition)
    {
        this.name = name;
        this.playerPosition = playerPosition;
    }
    
}
