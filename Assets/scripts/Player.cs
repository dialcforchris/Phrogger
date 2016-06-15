﻿using UnityEngine;
using System.Collections;

public class Player :WorldObject
{
    float angle;

    //private members
    private int strikes = 3;
    private int score = 0;

    //public members
    public int Score
    {
        get { return score; }
        set { score = value; }
    }
    public int Strikes
    {
        get { return strikes;}
        set {strikes= value;}
    }
    
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        Movement();
        MakeItBlue();
	}
    void Movement()
    {
       if (Input.GetAxis("Horizontal")!=0)
       {
           transform.position = TileManager.instance.GetTile(new Vector2((transform.position.x+Input.GetAxis("Horizontal")),transform.position.y)).transform.position;
           angle = Input.GetAxis("Horizontal") > 0 ? 270 : 90;
       }
       else if (Input.GetAxis("Vertical")!=0)
       {
           transform.position = TileManager.instance.GetTile(new Vector2(transform.position.x , (transform.position.y + Input.GetAxis("Vertical")))).transform.position;

           angle = Input.GetAxis("Vertical") > 0 ? 0 : 180;
       }
       transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    void MakeItBlue()
    {
        Tile tile = TileManager.instance.GetTile(transform.position);
        if (tile)
        {
            tile.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
    }
}
