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
    private WorkerState state;
    //move to chair logic
    List<Vector2> positions = new List<Vector2>();
    int targetIndex = 0;
    Vector2 chairFacing;
    float sitCool=0;
    float maxSitCool = 2;
    private int chairId =0;
    public bool goneToDesk = false;
    // help variables
    public GameObject helpMe;


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
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            //if (state == WorkerState.WALKING)
            //{ 
            //    Movement();
            //    animator.SetBool("walk", true);
            //}
            StateSwitch();
            Tile _tile = TileManager.instance.GetTile(transform.position);
            if (_tile != tiles[0])
            {
                RemoveFromWorld();
                AddToWorld();
                _tile.Interaction(this);
            }
        }
    }
    void Movement()
    {
        transform.position += direction * Time.deltaTime * speed;
     //   transform.rotation = Quaternion.Euler(direction);
     
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
    public void MoveToChair(Vector2 outide, Vector2 inside, Vector2 chairTile, Vector2 chairPivot)
    {
        positions.Add(outide);
        positions.Add(inside);
        positions.Add(chairTile);
        positions.Add(chairPivot);
        chairFacing = direction;
        state = WorkerState.SITTING;
        StopCoroutine("SitAtDesk");
        StartCoroutine("SitAtDesk");
    }

    IEnumerator SitAtDesk()
    {
        Vector2 currentTarget = positions[0];
        while(true)
        {
            if (Vector2.Distance(transform.position,currentTarget)<0.1f)
            {
                targetIndex++;
                if (targetIndex>=positions.Count)
                {
                    targetIndex = 0;
                    if (state == WorkerState.SITTING)
                    {
                        goneToDesk = true;
                        animator.SetBool("sit",true);
                        SatAtDesk();
                    }
                    yield break;
                }
                currentTarget = positions[targetIndex];
            }
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            Vector2 pos = transform.position;
            float lookAngle = Mathf.Atan2((transform.position.y - currentTarget.y), (transform.position.x - currentTarget.x)) * Mathf.Rad2Deg;
            Quaternion newRot = new Quaternion();
            newRot.eulerAngles = new Vector3(0, 0, lookAngle + 90);
            transform.rotation = newRot;
            yield return null;
        }
    }
    IEnumerator WalkFromDesk()
    {
        Vector2 currentTarget = positions[0];
        while (true)
        {
            if (Vector2.Distance(transform.position, currentTarget) < 0.1f)
            {
                targetIndex++;
                if (targetIndex ==positions.Count)
                {
                    targetIndex = 0 ;
                    state = WorkerState.WALKING;
                    if (goneToDesk)
                    {
                        goneToDesk = false; 
                    }
                    yield break;
                }
                currentTarget = positions[targetIndex];
            }
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            Vector2 pos = transform.position;
            float lookAngle = Mathf.Atan2((transform.position.y - currentTarget.y), (transform.position.x - currentTarget.x)) * Mathf.Rad2Deg;
            Quaternion newRot = new Quaternion();
            newRot.eulerAngles = new Vector3(0, 0, lookAngle + 90);
            transform.rotation = newRot;
            yield return null;
        }
    }
    void SatAtDesk()
    {
        if (Random.value>1.5f)
        {
       //     state = WorkerState.HELP;
        }
       
    }
    void StateSwitch()
    {
        switch (state)
        {
            case WorkerState.WALKING:
                {
                    animator.SetBool("walk",true);
                    animator.SetBool("sit", false);
                    

                    Movement();
                    break;
                }
            case WorkerState.HELP:
                {
                   if (!helpMe.activeSelf)
                   {
                       helpMe.SetActive(true);
                       helpMe.transform.rotation = Quaternion.Euler(Vector2.up);
                       helpMe.transform.position = (new Vector2(helpMe.transform.position.x , helpMe.transform.position.y + (transform.localScale.y*0.7f)));
                      
                    }
                    break;
                }
            case WorkerState.STANDING:
                {
                    animator.SetBool("walk",true);
                    animator.SetBool("sit", false);
                    break;
                }
            case WorkerState.SITTING:
                {

                    SitCooldown();
                    break;
                }
        }
    }
    void SitCooldown()
    {
        if (sitCool<maxSitCool)
        {
            sitCool += Time.deltaTime;
        }
        else
        {
            if (Random.value > 0.5f)
            {
                state = WorkerState.STANDING;
                StopCoroutine("WalkFromDesk");
                positions.Reverse();
                targetIndex = 0;
                StartCoroutine("WalkFromDesk");
            }
            else
            {
                state = WorkerState.HELP;
            }
        }
    }
}

