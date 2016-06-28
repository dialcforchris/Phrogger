using UnityEngine;
using System.Collections;

public class FroggerVehicle : FroggerObject
{
    protected override void Update()
    {
        base.Update();
        Tile _tile = TileManager.instance.GetTile(transform.position);
        if (_tile != tiles[0])
        {
            RemoveFromWorld();
            AddToWorld();
            _tile.Interaction(this);
        }
    }
}
