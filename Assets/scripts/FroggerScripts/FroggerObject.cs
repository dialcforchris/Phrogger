using UnityEngine;
using System.Collections;

public enum FroggerDeathType
{
    RUNOVER = 0,
    CROCO,
    DROWN,
    OFFSCREEN,
    COUNT
}


public class FroggerObject : WorldObject, IPoolable<FroggerObject>
{
    #region IPoolable
    public PoolData<FroggerObject> poolData { get; set; }
    #endregion

    [SerializeField] protected Transform[] pivots = null;
    protected int length;

    protected Vector3 direction;
    protected float speed;

    [SerializeField] protected bool killOnTouch = false;
    [SerializeField] protected FroggerDeathType deathType;

    public bool kill { get { return killOnTouch; } }
    public FroggerDeathType death { get { return deathType; } }

    protected override void Awake()
    {
        tiles = new Tile[pivots.Length];
        length = pivots.Length;
    }

    protected override void Start()
    {
       
    }

    public virtual void Initialise(Vector3 _dir, float _speed)
    {
        gameObject.SetActive(true);
        direction = _dir;
        speed = _speed;
        AddToWorld();
    }

    protected virtual void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_FROGGER)
        {
            transform.position += direction * Time.deltaTime * speed;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);

            Tile _tile = _tile = TileManager.instance.GetTile(pivots[0].position);
            if (_tile != tiles[0])
            {
                RemoveFromWorld();
                AddToWorld();
                for (int i = 0; i < pivots.Length; ++i)
                {
                    tiles[i].Interaction(this);
                }
            }
        }
        
    }

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Player")
        {
            if (killOnTouch)
            {
                ((Player)_obj).OriginalFroggerDeath(deathType);
            }
        }
    }

    public virtual void OffLevel()
    {
        Reset();
    }

    public override void AddToWorld()
    {
        for (int i = 0; i < pivots.Length; ++i)
        {
            Tile _tile = TileManager.instance.GetTile(pivots[i].position);
            _tile.Place(this);
            tiles[i] = _tile;
        }
    }

    public override void RemoveFromWorld()
    {
        for (int i = 0; i < pivots.Length; ++i)
        {
            tiles[i].Remove(this);
            tiles[i] = null;
        }
    }

    public override void Reset()
    {
        --length;
        if (length == 0)
        {
            length = pivots.Length;
            gameObject.SetActive(false);
            poolData.ReturnPool(this);
        }
    }
}
