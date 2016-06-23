using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum WorkerState
{
    WALKING,
    SITTING,
    STANDING,
    HELP,
}

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
    private WorkerState state;
    //move to chair logic
    private List<Vector2> positions = new List<Vector2>();
    private int targetIndex = 0;

    private float sitCool = 0;
    private float maxSitCool = 8.0f;
    private float maxSitLowerCool = 4.0f;
    private float maxSitUpperCool = 25.0f;
    private float standChance = 0.1f;

    private float helpCool = 0;
    private float maxHelpCool = 5.0f;
    private float helpChance = 0.05f;

    public bool hasEnteredCubicle { get; set; }

    public int chairId { get; set; }
    public int cubicleId { get; set; }

    public bool isJanitor { get; set; }


    private bool needHelp = false;
    public bool helpNeeded { get { return needHelp; } }

    // help variables
    [SerializeField] private GameObject helpMe = null;
    
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
            cubicleId = 100;
            isSetup = true;
            maxSitCool = Random.Range(maxSitLowerCool, maxSitUpperCool);
            isJanitor = _animName == "Janitor" ? true : false;
        }
    }

    public void SetupCubicle(int _cubicleId, int _chairId)
    {
        cubicleId = _cubicleId;
        chairId = _chairId;
    }

    public void Initialise(Vector3 _direction, float _speed)
    {
        direction = _direction;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, _direction == Vector3.left ? 90.0f : 270.0f));
        speed = _speed;
        AddToWorld();
        targetIndex = 0;
        gameObject.SetActive(true);
        hasEnteredCubicle = false;
    }

    public void InitialiseToCubicle(Vector3 _direction, float _speed)
    {
        Initialise(_direction, _speed);
        targetIndex = positions.Count - 1;

        StateSwitch(WorkerState.SITTING);
        animator.SetBool("sit", true);
        hasEnteredCubicle = true;
    }

    bool stopped;
    AnimatorClipInfo[] ipo;
    private void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (stopped)
            {
                animator.enabled = true;
                stopped = false;
            }
            StateUpdate();
        }
        else
        {
            animator.enabled = false;
            stopped = true;
        }
    }
    void Movement()
    {
        transform.position += direction * Time.deltaTime * speed;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        Tile _tile = TileManager.instance.GetTile(transform.position);
        if (_tile != tiles[0])
        {
            RemoveFromWorld();
            AddToWorld();
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
        if(_obj.tag == "Player")
        {
            if (state == WorkerState.WALKING || state == WorkerState.STANDING)
            {
                ((Player)_obj).Die();
            }
        }
    }

    //Whether an object can move to the same position as another object
    public override bool CheckMovement(WorldObject _obj)
    {
        if (_obj.tag == "Player")
        {
            if (state == WorkerState.SITTING || state == WorkerState.HELP)
            {
                return false;
            }
        }
        else if (_obj.tag == "Worker")
        {
            return false;
        }
        return true;
    }

    public override void Reset()
    {
        gameObject.SetActive(false);
        RemoveFromWorld();
        ReturnPool();
        helpCool = 0.0f;
        sitCool = 0.0f;
        targetIndex = 0;
        StopAllCoroutines();
        animator.SetBool("sit", false);
        StateSwitch(WorkerState.WALKING);
        needHelp = false;
        helpMe.SetActive(false);
    }

    public void ReturnPool()
    {
        poolData.ReturnPool(this);
    }

    public void AssignCubiclePivots(Vector2 outide, Vector2 inside, Vector2 chairTile, Vector2 chairPivot)
    {
        positions.Add(outide);
        positions.Add(inside);
        positions.Add(chairTile);
        positions.Add(chairPivot);
    }

    public void MoveToChair()
    {
        hasEnteredCubicle = true;
        StateSwitch(WorkerState.STANDING);
        //StopCoroutine("SitAtDesk");
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
                    targetIndex = positions.Count - 1;

                    animator.SetBool("sit",true);
                    StateSwitch(WorkerState.SITTING);

                    yield break;
                }
                currentTarget = positions[targetIndex];
            }
            transform.position = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            Tile _tile = TileManager.instance.GetTile(transform.position);
            if (_tile != tiles[0])
            {
                RemoveFromWorld();
                AddToWorld();
                _tile.Interaction(this);
            }
            float lookAngle = Mathf.Atan2((transform.position.y - currentTarget.y), (transform.position.x - currentTarget.x)) * Mathf.Rad2Deg;
            Quaternion newRot = new Quaternion();
            newRot.eulerAngles = new Vector3(0, 0, lookAngle + 90);
            transform.rotation = newRot;
            yield return null;
        }
    }
    IEnumerator WalkFromDesk()
    {
        Vector3 currentTarget = positions[positions.Count - 1];
        while (true)
        {
            if (Vector3.SqrMagnitude(currentTarget - transform.position) < 0.001f)
            {
                targetIndex--;
                if (targetIndex == -1)
                {
                    targetIndex = 0;
                    StateSwitch(WorkerState.WALKING);
                    yield break;
                }
                currentTarget = positions[targetIndex];
            }

            Vector3 _pos = Vector2.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
            Tile _t = TileManager.instance.GetTile(_pos);
            float lookAngle;
            Quaternion newRot;
            if (tiles[0] !=  _t)
            {
                if (!_t.CheckMovement(this))
                {
                    yield return null;
                    continue;
                }
                
                transform.position = _pos;
                RemoveFromWorld();
                AddToWorld();
                tiles[0].Interaction(this);
                lookAngle = Mathf.Atan2((transform.position.y - currentTarget.y), (transform.position.x - currentTarget.x)) * Mathf.Rad2Deg;
                newRot = new Quaternion();
                newRot.eulerAngles = new Vector3(0, 0, lookAngle + 90);
                transform.rotation = newRot;
                yield return null;
                continue;
            }

            transform.position = _pos;
            lookAngle = Mathf.Atan2((transform.position.y - currentTarget.y), (transform.position.x - currentTarget.x)) * Mathf.Rad2Deg;
            newRot = new Quaternion();
            newRot.eulerAngles = new Vector3(0, 0, lookAngle + 90);
            transform.rotation = newRot;

            yield return null;
        }
    }

    private void StateUpdate()
    {
        switch (state)
        {
            case WorkerState.WALKING:
                {
                    Movement();
                    break;
                }
            case WorkerState.HELP:
                {
                    break;
                }
            case WorkerState.STANDING:
                {
                    break;
                }
            case WorkerState.SITTING:
                {
                    SitCooldown();
                    break;
                }
        }
    }

    public void StateSwitch(WorkerState _newState)
    {
        state = _newState;
        sitCool = 0.0f;
        helpCool = 0.0f;
        switch (state)
        {
            case WorkerState.WALKING:
                {
                    animator.SetBool("walk",true);
                    break;
                }
            case WorkerState.HELP:
                {
                   if (!helpMe.activeSelf)
                    {
                        helpMe.SetActive(true);
                        helpMe.transform.rotation = Quaternion.Euler(Vector2.up);
                        helpMe.GetComponentInChildren<Animator>().Play("help_bob");
                        needHelp = true;
                    }
                    break;
                }
            case WorkerState.STANDING:
                {
                    animator.SetBool("sit", false);
                    animator.SetBool("walk",true);
                    break;
                }
            case WorkerState.SITTING:
                {
                    break;
                }
        }
    }
    void SitCooldown()
    {
        sitCool += Time.deltaTime;
        helpCool += Time.deltaTime;
        if (sitCool > maxSitCool)
        {
            sitCool = 0.0f;
            maxSitCool = Random.Range(maxSitLowerCool, maxSitUpperCool);
            if (Random.value < standChance)
            {
                StateSwitch(WorkerState.STANDING);
                //StopCoroutine("WalkFromDesk");
                StartCoroutine("WalkFromDesk");
                return;
            }
        }
        if (helpCool > maxHelpCool)
        {
            helpCool = 0.0f;
            if (Random.value < helpChance)
            {
                StateSwitch(WorkerState.HELP);
            }
        }
    }

    public void FinishedHelping()
    {
        needHelp = false;
        helpMe.SetActive(false);
        StateSwitch(WorkerState.SITTING);
    }
}

