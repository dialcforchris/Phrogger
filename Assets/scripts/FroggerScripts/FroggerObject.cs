using UnityEngine;
using System.Collections;

public enum FroggerDeathType
{
    RUNOVER = 0,
    CROCO,
    DROWN,
    COUNT
}


public class FroggerObject : WorldObject, IPoolable<FroggerObject>
{
    #region IPoolable
    public PoolData<FroggerObject> poolData { get; set; }
    #endregion

    private Vector3 direction;
    private float speed;

    [SerializeField] private bool killOnTouch = false;
    [SerializeField] private FroggerDeathType deathType;

    public bool kill { get { return killOnTouch; } }
    public FroggerDeathType death { get { return deathType; } }

    protected override void Start()
    {
       
    }

    public void Initialise(Vector3 _dir, float _speed)
    {
        gameObject.SetActive(true);
        direction = _dir;
        speed = _speed;
        AddToWorld();
    }

    public override void Reset()
    {
        gameObject.SetActive(false);
        poolData.ReturnPool(this);
    }

    protected virtual void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            transform.position += direction * Time.deltaTime * speed;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90, Vector3.forward) * direction);
        }
    }

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Player")
        {
            if (killOnTouch)
            {
                ((Player)_obj).OriginalFroggerDeath(deathType);
            }
        }
    }

    public virtual void OffLevel()
    {
        Reset();
    }
}
