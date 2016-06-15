using UnityEngine;
using System.Collections;

public class Player :WorldObject
{
    float angle;
    float coolDown = 0.6f;
    float maxCool = 0.6f;
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
        //transform.position = TileManager.instance.GetTile(transform.position).transform.position;
    }
	
	// Update is called once per frame
	void Update () 
    {
        Movement();
      //  MakeItBlue();
        MoveCooldown();
	}

    void Movement()
    {
        float move = 0;
        if (Input.GetAxis("Horizontal")!=0&&MoveCooldown())
        {
            move = Input.GetAxis("Horizontal") > 0 ? 1 : -1;

            Tile _tile = TileManager.instance.GetTile(new Vector2((transform.position.x + move), transform.position.y));
            if(_tile.CheckMovement(this))
            {
                transform.position = _tile.transform.position;
            }
            
            angle = Input.GetAxis("Horizontal") > 0 ? 270 : 90;
            coolDown = 0;
        }
        else if (Input.GetAxis("Vertical")!=0&&MoveCooldown())
        {
            move = Input.GetAxis("Vertical") > 0 ? 1 : -1;

            Tile _tile = TileManager.instance.GetTile(new Vector2(transform.position.x, (transform.position.y + move)));
            if (_tile.CheckMovement(this))
            {
                transform.position = _tile.transform.position;
            }

            angle = Input.GetAxis("Vertical") > 0 ? 0 : 180;
            coolDown = 0;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void MakeItBlue()
    {
        Tile tile = TileManager.instance.GetTile(transform.position);
        if (tile)
        {
            //tile.GetComponentInChildren<SpriteRenderer>().color = Color.blue;
        }
    }
    bool MoveCooldown()
    {
        if (coolDown<maxCool)
        {
            coolDown += Time.deltaTime;
            return false;
        }
        return true;
    }
}
