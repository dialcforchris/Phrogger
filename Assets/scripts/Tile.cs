using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    private int x, y;
    private List<WorldObject> objects = new List<WorldObject>();
    [SerializeField] private EnemyNode enemyNode = null;

    public void Initialise(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    private void Start()
    {
        TileManager.instance.CreateTileReference(x, y, this);
    }

    public void Place(WorldObject _obj)
    {
        objects.Add(_obj);
    }
    public void Remove(WorldObject _obj)
    {
        for(int i = 0; i < objects.Count; ++i)
        {
            if(objects[i] == _obj)
            {
                objects.RemoveAt(i);
            }
        }
    }

    public DirectionState NewDirection()
    {
        if(enemyNode)
        {
            return enemyNode.NewDirection();
        }
        return DirectionState.DIRECTION_COUNT;
    }

}
