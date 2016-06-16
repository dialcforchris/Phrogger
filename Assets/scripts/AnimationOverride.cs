using UnityEngine;
using System;

public class AnimationOverride : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    //private string lastSpriteName = string.Empty;
    private Sprite[] subSprites;//Array of sprites in the character's spritesheet


    ////Replace sprite animation with the correct spritesheet sprite - necessary so we can use a single animation controller/animations
    private void LateUpdate()
    {
        UpdateSprite();
    }

    public void SetSpriteSheet(string _spriteSheet)
    {
        subSprites = Resources.LoadAll<Sprite>("sprites/worker/" + _spriteSheet);
    }

    private void UpdateSprite()
    {
        string spriteName = spriteRenderer.sprite.name;
        
        //Find the name of the current sprite in the loaded spritesheet
        Sprite newSprite = Array.Find(subSprites, item => item.name == spriteName);
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        
    }
    
}
