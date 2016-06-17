using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour
{
    //The tiles the object is associated with
    [SerializeField] protected Tile[] tiles;
    [SerializeField] protected SpriteRenderer spriteRenderer = null;

    protected virtual void Awake()
    {
        tiles = new Tile[1];
    }

    protected virtual void Start()
    {
        AddToWorld();
    }

    public virtual void AddToWorld()
    {
        Tile _tile = TileManager.instance.GetTile(transform.position);
        _tile.Place(this);
        tiles[0] = _tile;
        
    }

    public virtual void RemoveFromWorld()
    {
        tiles[0].Remove(this);
        tiles[0] = null;
    }

    //The behavior of an object when something tries to interact with it
    public virtual void Interaction(WorldObject _obj)
    {

    }

    //Whether an object can move to the sam eposition as another object
    public virtual bool CheckMovement(WorldObject _obj)
    {
        return true;
    }

    public Tile GetTile(int _index)
    {
        return tiles[_index];
    }
}
