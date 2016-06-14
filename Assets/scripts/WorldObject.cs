using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour
{
    //The tiles the object is associated with
    protected Tile[] tile;

    protected virtual void Awake()
    {

    }

    //The behavior of an object when something tries to interact with it
    public virtual void Interaction(WorldObject _obj)
    {

    }


}
