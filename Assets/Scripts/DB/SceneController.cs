using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    //private SceneInfoController _sceneInfoController;
    //private PlayerInfoController playerInfoController;

    private SaveDataContorller saveDataContorller;

	void Start ()
	{
	    saveDataContorller = GameObject.FindGameObjectWithTag("Controller").GetComponent<SaveDataContorller>();
        Debug.Log(saveDataContorller);
	    //Loading scene data
	    //_sceneInfoController = transform.GetComponent<SceneInfoController>();
	    //var sceneInfo = _sceneInfoController.LoadScene(SceneManager.GetActiveScene().name);
	    //GameObject.FindGameObjectWithTag("Player").transform.position = sceneInfo.playerPosition;

	    //Loading player data
	    //playerInfoController = transform.GetComponent<PlayerInfoController>();
	    //playerInfoController.LoadController();

	}

    public void ChangeScene(string sceneName)
    {
        saveDataContorller.GetScene(SceneManager.GetActiveScene().name).playerPosition =
            GameObject.FindGameObjectWithTag("Player").transform.position;
        

        saveDataContorller.SaveDataToXML();

        SceneManager.LoadScene(sceneName);
    }
    
}
