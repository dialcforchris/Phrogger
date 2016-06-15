using UnityEngine;
using System.Collections;

public class Wall : WorldObject
{
    [SerializeField] private Sprite sprite = null;

    protected override void Awake()
    {
        spriteRenderer.sprite = sprite;
        base.Awake();
    }

    //The behavior of an object when something tries to interact with it
    public override void Interaction(WorldObject _obj)
    {

    }

    //Whether an object can move to the sam eposition as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return false;
    }
}
