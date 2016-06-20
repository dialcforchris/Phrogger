using UnityEngine;
using System.Collections;

public class FrogCorpse : WorldObject 
{
    public ParticleSystem blood;

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.GetComponent<Worker>())
        {
            blood.Play();
        }
    }
    public void Reset()
    {
        RemoveFromWorld();
    }
}
