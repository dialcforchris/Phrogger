using UnityEngine;
using System.Collections;

public class Cubicle : WorldObject 
{
    private enum Positions
    {
        OPENING = 0,
        TABLE,
        EMPTY,
        CHAIR,
        COUNT
    }

    public SpriteRenderer deskFodder;
    public Sprite[] tidyDesk;
    public Sprite[] messyDesk;

    private int currentDesk;

    [SerializeField] private Transform opening = null;
    [SerializeField] private Transform table = null;
    [SerializeField] private Transform empty = null;
    [SerializeField] private Transform[] chairs = null;
    [SerializeField] private Transform[] chairPivots = null;
    public bool[] filledChairs = null;

    private int deskId;
    public int cubicleId
    {
        get { return deskId; }
        set { deskId = value; }
    }
      
    
    private bool isMessy = false;

	// Use this for initialization
	protected override void Awake () 
    {
        tiles = new Tile[3 + chairs.Length];
        currentDesk = Random.Range(0, tidyDesk.Length);
        deskFodder.sprite = tidyDesk[currentDesk];

    }

    protected override void Start()
    {
        //if (transform.parent)
        //{
        //    Cubicle _o = (Cubicle)Instantiate(TileManager.instance.c, transform.position, transform.rotation);
        //    _o.name = "2X2Cubicle" + TileManager.instance.GetTile(transform.position).IndexName();
        //}
        Tile _tile = TileManager.instance.GetTile(opening.position);
        _tile.Place(this);
        tiles[(int)Positions.OPENING] = _tile;

        _tile = TileManager.instance.GetTile(table.position);
        _tile.Place(this);
        tiles[(int)Positions.TABLE] = _tile;

        _tile = TileManager.instance.GetTile(empty.position);
        _tile.Place(this);
        tiles[(int)Positions.EMPTY] = _tile;

        for (int i = 0; i < chairs.Length; ++i)
        {
            _tile = TileManager.instance.GetTile(chairs[i].position);
            _tile.Place(this);
            tiles[(int)Positions.CHAIR + i] = _tile;
        }
    }

    // Update is called once per frame
    void Update () 
    {
	    
	}

    //The behavior of an object when something tries to interact with it
    public override void Interaction(WorldObject _obj)
    {
        if(_obj.tag == "Worker")
        {
            if (_obj.GetTile(0) == tiles[(int)Positions.OPENING])
            {
                if (_obj.GetComponent<Worker>().cubicleId == deskId)
                {
                    _obj.GetComponent<Worker>().MoveToChair(opening.position,empty.position,chairs[0].position);
                }
                //Get their desk id, if it matches they enter
                //Move to empty space
                //Move to free chair
            }
        }
        else if(_obj.tag == "Player")
        {
            if (_obj.GetTile(0) == tiles[(int)Positions.TABLE])
            {
                if (!isMessy)
                {
                    isMessy = true;
                    deskFodder.sprite = messyDesk[currentDesk];
                }
                
                Phishing();
            }
            else if (_obj.GetTile(0) == tiles[(int)Positions.EMPTY])
            {
                Phishing();
            }
        }
    }

    private void Phishing()
    {
        //If player presses button to check email
        //If there is someone in need of help
        //Logic for if there is someone who needs help
    }

    //Whether an object can move to the same position as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return true;
    }
}
