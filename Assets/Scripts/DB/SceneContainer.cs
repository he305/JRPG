using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text;
using System.Xml.Serialization;

[XmlRoot("container")]
public class SceneContainer
{
    [XmlArray("scenes")]
    [XmlArrayItem("scene")]
    public List<SceneInfo> SceneInfos = new List<SceneInfo>();

    public void Add(SceneInfo info)
    {
        SceneInfos.Add(info);
    }

    public void SaveDataToXML()
    {
        var formatter = new XmlSerializer(typeof(SceneContainer));

        using (var sw = new StreamWriter(Application.dataPath + "/DB/scenes.xml", false, Encoding.UTF8))
        {
            formatter.Serialize(sw, this);
        }
    }

    public static SceneContainer LoadDataFromXML()
    {
        var formatter = new XmlSerializer(typeof(SceneContainer));

        try
        {
            using (var fs = new FileStream(Application.dataPath + "/DB/scenes.xml", FileMode.Open))
            {
                return formatter.Deserialize(fs) as SceneContainer;
            }
        }
        catch (FileNotFoundException ex)
        {
            return null;
        }
        catch (IsolatedStorageException ex)
        {
            return null;
        }
    }
}
