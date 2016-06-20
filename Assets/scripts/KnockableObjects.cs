using UnityEngine;
using System.Collections;

public class KnockableObjects : Wall
{
    public Sprite altImage;
	
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
