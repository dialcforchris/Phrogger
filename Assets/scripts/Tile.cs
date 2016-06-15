﻿using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private int x, y;
    [SerializeField] private List<WorldObject> objects = new List<WorldObject>();
    [SerializeField] private WorkerDirectionNode enemyNode = null;

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
    public virtual void Interaction(WorldObject _obj)
    {
        for(int i = 0; i < objects.Count; ++i)
        {
            objects[i].Interaction(_obj);
        }
    }

    //Whether an object can move to the same position as another object
    public virtual bool CheckMovement(WorldObject _obj)
    {
        for (int i = 0; i < objects.Count; ++i)
        {
            if(!objects[i].CheckMovement(_obj))
            {
                return false;
            }
        }
        return true;
    }

    public DirectionState NewDirection()
    {
        if(enemyNode)
        {
            return enemyNode.NewDirection();
        }
        return DirectionState.DIRECTION_COUNT;
    }

    public string IndexName()
    {
        return x.ToString() + ":" + y.ToString();
    }

    public void UpdateSprite(Sprite _sprite)
    {
        spriteRenderer.sprite = _sprite;
    }

}
