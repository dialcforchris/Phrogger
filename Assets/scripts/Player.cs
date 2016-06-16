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
    private float verti;
    Animator ani;
    PlayerState state;

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
        ani = GetComponent<Animator>();
        state = PlayerState.ACTIVE;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (state == PlayerState.ACTIVE)
        {
            ConvertToPos();
            Movement();
            MoveCooldown();
            Die();
        }
       
       
	}

    void Movement()
    {
        float moveX = 0;
        float moveY = 0;

        if (hori > verti)
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

        ani.SetBool("PlayerWalk", ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && !MoveCooldown()) ? true : false);
        
    }

    void MakeItBlue()
    {
        Tile tile = TileManager.instance.GetTile(transform.position);
        if (tile)
        {
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
    //movement related function
    void ConvertToPos()
    {
        hori = Input.GetAxis("Horizontal");
        verti = Input.GetAxis("Vertical");
        if (hori < 0)
        {
            hori -= hori * 2;
        }
        if (verti < 0)
        {
            verti -= verti * 2;
        }
    }
  public void Die()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            state = PlayerState.DEAD;
            ani.SetTrigger("Dead");
            strikes -= 1;
        }
    }
  public override void Interaction(WorldObject _obj)
  {
      if (_obj.tag == "Worker")
      {
          Die();
      }
      
  }

}
enum PlayerState
{
    ACTIVE,
    DEAD,
}
