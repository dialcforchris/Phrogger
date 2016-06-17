using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worker : WorldObject, IPoolable<Worker>
{
    #region IPoolable
    public PoolData<Worker> poolData { get; set; }
    #endregion

    enum WorkerState
    {
        WALKING,
        SITTING,
        STANDING,
        HELP,
    }

    private bool isSetup = false;
    [SerializeField] private SpriteRenderer hairSpriteRenderer = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private AnimationOverride animOverride= null;
    private int deskId;
    private Vector3 direction;
    private float speed = 5.0f;
    
    //move to chair logic
    List<Vector2> positions = new List<Vector2>();
    int targetIndex = 0;
    bool finishedMovement;

    public int cubicleId
    {
        get { return deskId; }
        set { deskId = value; }
    }
    protected override void Start()
    {
        //Override to prevent base start getting called
        
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
        AddToWorld();
        gameObject.SetActive(true);
    }

    private void Update()
    {
        Movement();
        Tile _tile = TileManager.instance.GetTile(transform.position);
        if (_tile != tiles[0])
        {
            RemoveFromWorld();
            AddToWorld();
            _tile.Interaction(this);
        }
    }
    void Movement()
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

    //Whether an object can move to the same position as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        return true;
    }

    public void Reset()
    {
        RemoveFromWorld();
        poolData.ReturnPool(this);
        gameObject.SetActive(false);
    }
    public void MoveToChair(Vector2 outide, Vector2 inside, Vector2 chair)
    {
        positions.Add(outide);
        positions.Add(inside);
        positions.Add(chair);
        StartCoroutine("SitAtDesk");
        Debug.Log("moving to chair " + deskId);

    }
    IEnumerable SitAtDesk()
    {
        Vector2 currentTarget = positions[0];
        while(true)
        {
            if (Vector2.Distance(transform.position,currentTarget)<0.1f)
            {
                targetIndex++;
                if (targetIndex>=positions.Count)
                {
                    finishedMovement = true;
                    targetIndex = 0;
                    yield break;
                }
                currentTarget = positions[targetIndex];
            }
        }
       // transform.position = Vector2.MoveTowards(transform.position,currentTarget,speed*Time.deltaTime);
       // yield return null;
    }
}

