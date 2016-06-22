using UnityEngine;
using System.Collections;

public class FrogCorpse : WorldObject 
{
    public ParticleSystem blood;

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Worker" || _obj.tag == "Boss")
        {
            blood.Play();
        }
    }
    public override void Reset()
    {
        RemoveFromWorld();
        gameObject.SetActive(false);
    }
}
