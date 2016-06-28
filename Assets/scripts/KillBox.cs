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
    [SerializeField] private  KillBoxAffects killBoxAffects = KillBoxAffects.EVERYTHING;

    public override void Interaction(WorldObject _obj)
    {
        switch(killBoxAffects)
        {
            case KillBoxAffects.PLAYER:
                if (_obj.tag == "Player")
                {
                    ((Player)_obj).Die();
                }
                break;
            case KillBoxAffects.WORKER:
                if(_obj.tag == "Worker")
                {
                    ((Worker)_obj).Reset();
                }
                break;
            case KillBoxAffects.EVERYTHING:
                if (_obj.tag == "Player")
                {
                    ((Player)_obj).Die();
                }
                else if (_obj.tag == "Worker")
                {
                    ((Worker)_obj).Reset();
                }
                else if (_obj.tag == "FroggerObject")
                {
                    ((FroggerObject)_obj).OffLevel();
                }
                break;
            default:
                break;
        }
    }
}
