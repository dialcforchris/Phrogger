using UnityEngine;
using System.Collections;

public enum KillBoxAffects
{
    PLAYER = 0,
    WORKER,
    EVERYTHING,
    COUNT
}

public class KillBox : WorldObject
{
    [SerializeField] private  KillBoxAffects killBoxAffects;

    public override void Interaction(WorldObject _obj)
    {
        switch(killBoxAffects)
        {
            case KillBoxAffects.PLAYER:
                if (_obj.tag == "Player")
                {
                    _obj.Remove();
                }
                break;
            case KillBoxAffects.WORKER:
                if(_obj.tag == "Worker")
                {
                    _obj.Remove();
                }
                break;
            case KillBoxAffects.EVERYTHING:
                _obj.Remove();
                break;
            default:
                break;
        }
    }
}
