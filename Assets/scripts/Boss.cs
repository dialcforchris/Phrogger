using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : WorldObject
{
    private static Boss boss = null;
    public static Boss instance { get { return boss; } }

    [SerializeField] private AnimationOverride animOverride= null;

    private List<Tile> tileSearch = new List<Tile>();

    [SerializeField] private float speed = 2.0f;

    [SerializeField] private Player player = null;

    private bool chasePlayer = false;
    

    protected override void Awake()
    {
        base.Awake();
        boss = this;
        animOverride.SetSpriteSheet("The Boss");
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        
    }

    private void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (tileSearch.Count != 0)
            {
                
                transform.position = Vector3.MoveTowards(transform.position, tileSearch[0].transform.position, Time.deltaTime * speed);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - tileSearch[0].transform.position);
                if (Vector3.SqrMagnitude(tileSearch[0].transform.position - transform.position) < 0.05f)
                {
                    tileSearch.RemoveAt(0);
                    for (int i = 2; i < 5; ++i)
                    {
                        if (i >= tileSearch.Count)
                        {
                            break;
                        }
                        else if (tileSearch[0] == tileSearch[i])
                        {
                            for (int j = i; j > 0; --j)
                            {
                                tileSearch.RemoveAt(j);
                            }
                            break;
                        }
                    }
                }
                
                Tile _tile = TileManager.instance.GetTile(transform.position);
                if (_tile != tiles[0])
                {
                    RemoveFromWorld();
                    AddToWorld();
                    _tile.Interaction(this);
                }
            }
            else
            {
                //Return boss to portrait here
                if (!chasePlayer)
                {
                    RemoveFromWorld();
                    gameObject.SetActive(false);
                }
            } 
        }
    }
   
    private void LateUpdate()
    {
        animOverride.UpdateSprite();
    }

    //The behavior of an object when something tries to interact with it
    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Player")
        {
            EndChase();
            ((Player)_obj).Die();
        }
    }

    //Whether an object can move to the same position as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return true;
    }

    public void BeginChase()
    {
        tileSearch = player.GetRouteToPlayer();
        transform.position = tileSearch[0].transform.position;
        AddToWorld();
        chasePlayer = true;
        gameObject.SetActive(true);
    }

    public void EndChase()
    {
        if (chasePlayer)
        {
            tileSearch.Clear();
            chasePlayer = false;
            GetRouteToExit();
        }
    }

    public void ChasePlayer(Tile _tile)
    {
        if (chasePlayer)
        {
            tileSearch.Add(_tile);
        }
    }

    private void GetRouteToExit()
    {
        int _x;
        int _y;
        int _direction;
        Tile _exit;

        //Loop through objects to see if player is in cubicle
        foreach (WorldObject _wo in tiles[0].GetObjects())
        {
            if (_wo.tag == "Cubicle")
            {
                //Create a tile route out the cubicle
                tileSearch = ((Cubicle)_wo).GetTileRoute(tiles[0]);
                tileSearch.Reverse();
                //Get the closest spawner
                _exit = TileManager.instance.GetAssociatedSpawner(tileSearch[tileSearch.Count - 1]);

                _x = tileSearch[tileSearch.Count - 1].X();
                _y = tileSearch[tileSearch.Count - 1].Y();
                _direction = _x > _exit.X() ? 1 : -1;

                //Loop and create route from spawner to entrance of cubicle
                while (true)
                {
                    _x += _direction;
                    Tile _tempTile = TileManager.instance.GetTile(_x, _y);
                    if (_tempTile.Y() != _exit.Y())
                    {
                        return;
                    }
                    tileSearch.Add(_tempTile);
                }
            }
        }

        _exit = TileManager.instance.GetAssociatedSpawner(tiles[0]);

        _x = tiles[0].X();
        _y = tiles[0].Y();
        _direction = _y < _exit.Y() ? 1 : -1;

        while (true)
        {
            Tile _tempTile = TileManager.instance.GetTile(_x, _y);
            tileSearch.Add(_tempTile);
            if (_tempTile.Y() == _exit.Y())
            {
                _direction = _x > _exit.X() ? 1 : -1;

                while (true)
                {
                    _x += _direction;
                    _tempTile = TileManager.instance.GetTile(_x, _y);
                    if (_tempTile.Y() == _exit.Y())
                    {
                        tileSearch.Add(_tempTile);
                        continue;
                    }
                    return;
                }
            }
            _y += _direction;  
        }
    }

}
