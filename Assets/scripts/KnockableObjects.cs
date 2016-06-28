using UnityEngine;
using System.Collections;

public class KnockableObjects : Wall
{
    public Sprite altImage;
    public bool reversable;
    [SerializeField]
    private AudioClip knockedSound;
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
            SoundManager.instance.playSound(knockedSound);
        }
        else
        {
            if (spriteRenderer.sprite != altImage)
            {
                spriteRenderer.sprite = altImage;
                StatTracker.instance.messyDesks++;
                SoundManager.instance.playSound(knockedSound);
            }
        }
    }
    public override bool CheckMovement(WorldObject _obj)
    {
        Knocked();
        return base.CheckMovement(_obj);
    }

    public override void Reset()
    {
        spriteRenderer.sprite = image;
    }
}
