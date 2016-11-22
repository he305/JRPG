using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(transform.parent.position.x, transform.parent.position.y, -10);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }
}
