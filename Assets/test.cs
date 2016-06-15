using UnityEngine;
using System.Collections;

public class test : MonoBehaviour 
{
    bool moveDir;
	// Use this for initialization
	void Start ()
    {
        moveDir = Camera.main.WorldToViewportPoint(transform.position).x >0 ? true:false;
       
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (moveDir)
        {
            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(Camera.main.ViewportToWorldPoint(new Vector2(-0.1f, 0)).x,transform.position.y),Time.deltaTime*10);
            if (Camera.main.WorldToViewportPoint(transform.position).x<=0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position,
               new Vector2(Camera.main.ViewportToWorldPoint(new Vector2(1.1f, 0)).x, transform.position.y), Time.deltaTime*10);
            if (Camera.main.WorldToViewportPoint(transform.position).x>=1)
            {
                Destroy(gameObject);
            }
        }
        
	}
}
