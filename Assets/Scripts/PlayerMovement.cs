using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 10;
    private Rigidbody2D rb;

	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed,
                                          Input.GetAxisRaw("Vertical") * moveSpeed);

        /*
        transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime,
                                          Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime);
	    */
    }
}
