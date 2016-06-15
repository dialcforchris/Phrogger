using UnityEngine;
using System.Collections;

public class WorldObject : MonoBehaviour
{
    //The tiles the object is associated with
    protected Tile[] tile;
    [SerializeField] protected SpriteRenderer spriteRenderer = null;

    protected virtual void Awake()
    {

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


}
