using UnityEngine;
using System.Collections;

public class Wall : WorldObject
{
    [SerializeField] private bool blockUp = true;
    [SerializeField] private bool blockDown = true;
    [SerializeField] private bool blockLeft = true;
    [SerializeField] private bool blockRight = true;

    [SerializeField] private bool inverted = false;

    protected override void Awake()
    {
        transform.position = new Vector3(Mathf.Floor(transform.position.x) + 0.5f, Mathf.Round(transform.position.y), 0.0f);

        base.Awake();

        if (inverted)
        {
            bool _temp = blockUp;
            blockUp = blockDown;
            blockDown = _temp;
            _temp = blockLeft;
            blockLeft = blockRight;
            blockRight = _temp;
        }
    }

    //The behavior of an object when something tries to interact with it
    public override void Interaction(WorldObject _obj)
    {

    }

    //Whether an object can move to the sam eposition as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        if(blockUp)
        {
            if(transform.position.y > _obj.transform.position.y)
            {
                return false;
            }
        }
        if(blockDown)
        {
            if (transform.position.y < _obj.transform.position.y)
            {
                return false;
            }
        }
        if(blockLeft)
        {
            if (transform.position.x < _obj.transform.position.x)
            {
                return false;
            }
        }
        if(blockRight)
        {
            if (transform.position.x > _obj.transform.position.x)
            {
                return false;
            }
        }
        return true;
    }
}
