using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : WorldObject
{
    private static Boss boss = null;
    public static Boss instance { get { return boss; } }

    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationOverride animOverride= null;

    private List<Tile> tileSearch = new List<Tile>();

    private float speed = 2.0f;
    

    protected override void Awake()
    {
        base.Awake();
        boss = this;
        animOverride.SetSpriteSheet("The Boss");
        gameObject.SetActive(false);
    }

    protected override void Start()
    {
        AddToWorld();
    }

    private void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (tileSearch.Count != 0)
            {
                transform.position = Vector3.MoveTowards(transform.position, tileSearch[0].transform.position, Time.deltaTime * speed);
                transform.rotation = Quaternion.LookRotation(Vector3.forward, tileSearch[0].transform.position - transform.position);
                if (Vector3.SqrMagnitude(tileSearch[0].transform.position - transform.position) < 0.05f)
                {
                    tileSearch.RemoveAt(0);
                    for(int i = 2; i < 5; ++i)
                    {
                        if(i >= tileSearch.Count)
                        {
                            break;
                        }
                        else if(tileSearch[0] == tileSearch[i])
                        {
                            for(int j = i; j > 0; --j)
                            {
                                tileSearch.RemoveAt(j);
                            }
                            break;
                        }
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
            ((Player)_obj).Die();
        }
    }

    //Whether an object can move to the same position as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return true;
    }

    public void ChasePlayer(Tile _tile)
    {
        tileSearch.Add(_tile);
    }

}
