using UnityEngine;
using System.Collections;

public class FrogCorpse : WorldObject 
{
    public ParticleSystem blood;

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
            }
        }
        else if (_obj.tag == "Boss")
        {
            blood.Play();
        }
    }
    public override void Reset()
    {
        RemoveFromWorld();
        Destroy(gameObject);
    }
}
