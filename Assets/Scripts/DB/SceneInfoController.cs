using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SceneInfoController : MonoBehaviour
{

    private SceneContainer sceneContainer;

	// Use this for initialization
	public SceneInfo LoadScene (string scene)
    {

        if (SceneContainer.LoadDataFromXML() != null)
        {
            sceneContainer = SceneContainer.LoadDataFromXML();
            foreach (var sceneInfo in sceneContainer.SceneInfos)
            {
                if (sceneInfo.name == scene)
                    return sceneInfo;
            }
        }

        sceneContainer = new SceneContainer();
        sceneContainer.Add(new SceneInfo(scene, GameObject.FindGameObjectWithTag("Player").transform.position));
	    return sceneContainer.SceneInfos[0];
    }

    public SceneInfo GetScene(string scene)
    {
        foreach (var sceneInfo in sceneContainer.SceneInfos)
        {
            if (sceneInfo.name == scene)
                return sceneInfo;
        }
        return null;
    }

    public List<SceneInfo> GetAllScenes()
    {
        return sceneContainer.SceneInfos;
    }

    public void LoadDataToXML(List<SceneInfo> info)
    {
        sceneContainer.SceneInfos = info;
        sceneContainer.SaveDataToXML();
    }
}
