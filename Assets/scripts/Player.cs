using UnityEngine;
using System.Collections;

public class Player :WorldObject
{
    float angle;
    float coolDown = 0.6f;
    float maxCool = 0.4f;
    Vector2 lastPos;
    //private members
    private int strikes = 3;
    private int score = 0;
    private float hori;
    private float veri;

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
        lastPos = Vector2.zero;
        //transform.position = TileManager.instance.GetTile(transform.position).transform.position;
    }
	
	// Update is called once per frame
	void Update () 
    {
        ConvertToPos();
        Movement();
        MoveCooldown();
      
	}

    void Movement()
    {
        float moveX = 0;
        float moveY = 0;
        if (hori > veri)
        {
            if (Input.GetAxis("Horizontal") != 0 && MoveCooldown())
            {
                moveX = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
                if (lastPos.x != moveX)
                {
                    Tile _tile = TileManager.instance.GetTile(new Vector2((transform.position.x + moveX), transform.position.y));
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _tile.transform.position;
                    }

                    angle = Input.GetAxis("Horizontal") > 0 ? 270 : 90;
                    coolDown = 0;
                    lastPos.x = moveX;
                }
            }
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0 && MoveCooldown())
            {
                moveY = Input.GetAxis("Vertical") > 0 ? 1 : -1;
                if (lastPos.y != moveY)
                {
                    Tile _tile = TileManager.instance.GetTile(new Vector2(transform.position.x, (transform.position.y + moveY)));
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _tile.transform.position;
                    }

                    angle = Input.GetAxis("Vertical") > 0 ? 0 : 180;
                    coolDown = 0;
                    lastPos.y = moveY;
                }
            }
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (Input.GetAxis("Horizontal")==0)
        {
            lastPos.x = 0;
        }
        if (Input.GetAxis("Vertical")==0)
        {
            lastPos.y = 0;
        }
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
    void ConvertToPos()
    {
        hori = Input.GetAxis("Horizontal");
        veri = Input.GetAxis("Vertical");
        if (hori<0)
        {
            hori -= hori * 2;
        }
        if (veri<0)
            {
            veri -= veri*2;
        }
            }
  
}
