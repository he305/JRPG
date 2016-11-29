using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveDataContorller : MonoBehaviour
{

    private SaveData data;

    void Start ()
    {
        data = SaveData.LoadDataFromXML("current");
        var sceneInfo = data.GetSceneByName(SceneManager.GetActiveScene().name);

        if (sceneInfo != null)
        {
            GameObject.FindGameObjectWithTag("Player").transform.position = sceneInfo.playerPosition;
            data.currentScene = SceneManager.GetActiveScene().name;
        }
    }

    public void ChangePlayerInfo(List<PlayerInfo> playerInfos)
    {
        data.playerInfoContainer = playerInfos;
        SaveDataToXML();
    }

    public void CreateNewSave(string saveName)
    {
        data.SaveDataToXML(saveName);
    }

    public SceneInfo GetScene(string nameScene)
    {
        return data.GetSceneByName(nameScene);
    }

    public PlayerInfo GetPlayerInfo(int i)
    {
        return data.GetPlayerInfo(i);
    }

    public void SaveDataToXML()
    {
        data.SaveDataToXML("current");
    }
}
