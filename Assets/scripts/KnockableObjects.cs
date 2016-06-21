using UnityEngine;
using System.Collections;

public class KnockableObjects : Wall
{
    public Sprite altImage;
    Sprite image;
    protected override void Start()
    {
        base.Start();
        image = spriteRenderer.sprite;
    }
    public void Knocked()
    {
        spriteRenderer.sprite = altImage;
    }
    public override bool CheckMovement(WorldObject _obj)
    {
        Knocked();
        return base.CheckMovement(_obj);
    }
}
