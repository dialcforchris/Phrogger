using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
    float angle;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        Movement();
	}
    void Movement()
    {
       if (Input.GetAxis("Horizontal")!=0)
       {
           transform.position = new Vector2(transform.position.x + Input.GetAxis("Horizontal"),transform.position.y);

       }
    }
}
