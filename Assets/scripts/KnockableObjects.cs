using UnityEngine;
using System.Collections;

public class KnockableObjects : Wall
{
    public Sprite altImage;
    public bool reversable;
    bool alternateSprites;
    Sprite image;
    protected override void Start()
    {
        base.Start();
        image = spriteRenderer.sprite;
    }
    public void Knocked()
    {
        if (reversable)
        {
            spriteRenderer.sprite = alternateSprites ? image : altImage;
            alternateSprites = !alternateSprites;
        }
        else
        {
            spriteRenderer.sprite = altImage;
           StatTracker.instance.messyDesks++;
        }
    }
    public override bool CheckMovement(WorldObject _obj)
    {
        Knocked();
        return base.CheckMovement(_obj);
    }
}
