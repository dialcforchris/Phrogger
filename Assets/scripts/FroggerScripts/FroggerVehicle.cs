using UnityEngine;
using System.Collections;

public class FroggerVehicle : FroggerObject
{
    [SerializeField] private Transform[] pivots = null;
    private int length;

    protected override void Awake()
    {
        tiles = new Tile[pivots.Length];
        length = pivots.Length;
    }

    protected override void Update()
    {
        base.Update();
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
        if(length == 0)
        {
            length = pivots.Length;
            gameObject.SetActive(false);
            poolData.ReturnPool(this);
        }  
    }
}
