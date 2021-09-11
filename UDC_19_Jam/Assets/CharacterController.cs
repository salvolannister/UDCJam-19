using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    public static float height = 2.89f; // screen borders 
    [SerializeField]
    public static float width = 5.67f;  // screen borders

    public float fallTime = 0.5f;
    public float speed = 2;
    private float xMov = 0;
    private float yMov = 0;
    private float timeRead = 0;
    private bool hasReachedDestination;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        xMov = Input.GetAxisRaw("Horizontal");
        yMov = Input.GetAxisRaw("Vertical");

        if (xMov != 0)
        {
            float delta = xMov * Time.deltaTime * speed;
           
            transform.position += new Vector3(delta, 0, 0);
            if (transform.position.x > width || transform.position.x < 0)
            {
                transform.position -= new Vector3(delta, 0, 0);
            }
        }
        
        if(Time.realtimeSinceStartup - timeRead > (yMov < 0 ? fallTime/10 : fallTime))
        {
            timeRead = Time.realtimeSinceStartup;
            if(transform.position.y > 0)
            {
                transform.position += new Vector3(0, -1, 0);
            }
        }

    }
}
