using UnityEngine;
using System.Collections;

public class Worker : WorldObject, IPoolable<Worker>
{
    #region IPoolable
    public PoolData<Worker> poolData { get; set; }
    #endregion

    private bool isSetup = false;
    [SerializeField] private SpriteRenderer hairSpriteRenderer = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationOverride animOverride= null;

    private void Awake()
    {

    }

    public void SetupWorker(string _animName, Sprite _hairSprite)
    {
        if(!isSetup)
        {
            hairSpriteRenderer.sprite = _hairSprite;
            animOverride.SetSpriteSheet(_animName);
        }
    }

    public void Initialise()
    {
        gameObject.SetActive(true);
    }
    //The behavior of an object when something tries to interact with it
    public override void Interaction(WorldObject _obj)
    {

    }

    //Whether an object can move to the sam eposition as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return true;
    }

    public void Reset()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
