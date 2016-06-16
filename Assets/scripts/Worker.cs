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

    private Vector3 direction;
    private float speed = 5.0f;

    private void Awake()
    {
   
    }

    public bool GetIsSetup()
    {
        return isSetup;
    }

    public void SetupWorker(string _animName, Sprite _hairSprite)
    {
        if(!isSetup)
        {
            hairSpriteRenderer.sprite = _hairSprite;
            animOverride.SetSpriteSheet(_animName);
        }
    }

    public void Initialise(Vector3 _direction, float _speed)
    {
        direction = _direction;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, _direction == Vector3.left ? 90.0f : 270.0f));
        speed = _speed;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    private void LateUpdate()
    {
        animOverride.UpdateSprite();
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

    public override void Remove()
    {
        Reset();
    }

    public void Reset()
    {
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
