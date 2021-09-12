using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    public static float height = 3f; // screen borders 
    [SerializeField]
    public static float width = 6f;  // screen borders

    public float fallTime = 0.5f;
    public float speed = 3;
    private float xMov = 0;
    private float yMov = 0;
    private float timeRead = 0;

    public bool hasReachedDestination;
    private readonly float offSet = 0.15f;
    [HideInInspector]
    public Character character;
    private bool moving = false;
    // Start is called before the first frame update

    private void Awake()
    {
        hasReachedDestination = false;
        moving = true;
    }
    void Start()
    {
        GetWidth();
    }

    private void GetWidth()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasReachedDestination)
        {
            xMov = Input.GetAxisRaw("Horizontal");
            yMov = Input.GetAxisRaw("Vertical");

            if (xMov != 0)
            {
                float delta = xMov * Time.deltaTime;
                
                transform.position += new Vector3(delta, 0, 0);
                if (transform.position.x > width || transform.position.x < 0 || !CheckIfPositionIsValid())
                {
                    transform.position -= new Vector3(delta, 0, 0);
                }
                else
                {
                    moving = true;
                }
            }

            if (Time.realtimeSinceStartup - timeRead > (yMov < 0 ? fallTime / 10 : fallTime))
            {
                timeRead = Time.realtimeSinceStartup;
                transform.position += new Vector3(0, -1, 0);
                if (transform.position.y < 0 || !CheckIfPositionIsValid())
                {
                    transform.position -= new Vector3(0, -1, 0);
                    moving = false;
                }
               

            }

            CheckIfDestIsReached();
        }



    }

    private void CheckIfDestIsReached()
    {
        if (transform.position.y <= 0 + offSet  || !moving)
        {
            hasReachedDestination = true;
            // RoundX pos
            float roundedX = Mathf.Round(transform.position.x);
            float roundedY = Mathf.Round(transform.position.y);
            transform.position = new Vector3(roundedX, roundedY, 0);
            if(GatherYourPeople.AddCharacterLocation(transform.position, this))
            {
                Debug.Log(" added  transform position " + transform.position);
            }
            
        }
      

    }

    private bool CheckIfPositionIsValid()
    {
        return GatherYourPeople.IsPositionValid(transform.position);
       
    }
}
