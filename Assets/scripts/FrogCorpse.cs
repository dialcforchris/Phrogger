using UnityEngine;
using System.Collections;

public class FrogCorpse : WorldObject 
{
    public ParticleSystem blood;
    [SerializeField]
    private AudioClip splat;

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
                blood.Play();
                SoundManager.instance.playSound(splat);
            }
        }
        else if (_obj.tag == "Boss")
        {
            SoundManager.instance.playSound(splat);
            blood.Play();
        }
    }
    public override void Reset()
    {
        RemoveFromWorld();
        Destroy(gameObject);
    }
}
