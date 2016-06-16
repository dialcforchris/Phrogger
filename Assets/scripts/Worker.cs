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
        tiles = new Tile[1];
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
        Tile _tile = TileManager.instance.GetTile(transform.position);
        _tile.Place(this);
        tiles[0] = _tile;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
        Tile _tile = TileManager.instance.GetTile(transform.position);
        if (_tile != tiles[0])
        {
            tiles[0].Remove(this);
            _tile.Place(this);
            tiles[0] = _tile;
            _tile.Interaction(this);
        }
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
        tiles[0].Remove(this);
        tiles[0] = null;
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
}
