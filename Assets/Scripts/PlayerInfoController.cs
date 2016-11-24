using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Assets.Scripts;

public class PlayerInfoController : MonoBehaviour {
    // Use this for initialization
    void Start () {

        PlayerInfoContainer container = new PlayerInfoContainer();

        container.Add(new PlayerInfo("Vano", 100, 20));
        container.Add(new PlayerInfo("sadasd", 1002, 230));
        container.SaveDataToXML();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
