using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FrogCorpse : WorldObject 
{
    public ParticleSystem blood;
    [SerializeField]
    private AudioClip splat;
    float stomps = 1;

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Worker")
        {
            if (((Worker)_obj).isJanitor)
            {
                Reset();
            }
            else
            {
                Alpha();
            }
        }
        else if (_obj.tag == "Boss" || _obj.tag == "FroggerObject")
        {
            Alpha();
        }
    }
    public override void Reset()
    {
        RemoveFromWorld();
        Destroy(gameObject);
    }
 
    void Alpha()
    {
        stomps -= 0.1f;
        if (stomps > 0)
        {
            blood.Play();
        }
        if (stomps >=0)
        {
            spriteRenderer.color = new Color(1, 1, 1, stomps);
            SoundManager.instance.playSound(splat, stomps);
        }
    }
}
