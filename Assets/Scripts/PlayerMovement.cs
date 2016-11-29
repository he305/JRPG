using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float distance;
    private Vector2 lastPos;

    public float moveSpeed = 50;
    private Rigidbody2D rb;

    // Use this for initialization
    private void Start()
    {
        lastPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        distance += Vector2.Distance(transform.position, lastPos);

        lastPos = transform.position;

        if (distance >= 300)
        {
            distance = 0;
            rb.velocity = Vector2.zero;

            GameObject.FindGameObjectWithTag("Controller").GetComponent<SceneController>().ChangeScene("battle");

            return;
        }

        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal")*moveSpeed,
            Input.GetAxisRaw("Vertical")*moveSpeed);

        /*
        transform.position += new Vector3(Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime,
                                          Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime);
	    */
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Saved!!!");
            other.gameObject.GetComponent<Interactable>().DoAction();
        }
    }
}