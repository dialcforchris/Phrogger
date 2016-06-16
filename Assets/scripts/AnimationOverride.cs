using UnityEngine;
using System;

public class AnimationOverride : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer = null;

    private Sprite[] subSprites;

    public void SetSpriteSheet(string _spriteSheet)
    {
        subSprites = Resources.LoadAll<Sprite>("Workers/" + _spriteSheet);
        spriteRenderer.sprite = subSprites[0];
        Debug.Log(subSprites.Length);
    }

    public void UpdateSprite()
    {
        if(subSprites == null)
        {
            return;
        }

        string spriteName = spriteRenderer.sprite.name;
        
        Sprite newSprite = Array.Find(subSprites, item => item.name == spriteName);
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
