﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : WorldObject
{
    public static Player instance;

    float angle;
    float coolDown = 0.6f;
    float maxCool = 0.4f;
    private Vector2 lastPos = Vector2.zero;
    //private members
    public int strikes = 3;
    private int score = 0;
    private float hori;
    private float verti;
    public ParticleSystem bloodSplatter;
    [SerializeField] private FrogCorpse corpse;
    [SerializeField]
    private AudioClip splat;
    [SerializeField] AudioClip splash;
    public bool joyOrDPad = false; //true for joy, false for dpad

    [SerializeField]
    private ParticleSystem respawnParticles;

    [SerializeField] private Transform playerSpawn = null;
    [SerializeField] private Transform froggerSpawn = null;
    private bool froggerCompleted = false;
    
    [SerializeField] private Animator anim = null;
    private PlayerState state = PlayerState.ACTIVE;
    public PlayerState playerState
    {
        get { return state; }
    }
    float deathCool = 0;
    float maxDeathcool = 3;

    [SerializeField] private Vector2 waterZone = Vector2.zero; //This is dirty, I know a way around it, I do not care, this is the only water zone

    //public members
    public int Score
    {
        get { return score; }
        set { score = value; }
    }
    public int Strikes
    {
        get { return strikes; }
        set { strikes = value; }
    }

    protected override void Awake()
    {
        instance = this;
        base.Awake();
        transform.position = froggerSpawn.position;
    }
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            coolDown = 0.0f;
            froggerCompleted = true;
            respawnParticles.transform.position = playerSpawn.position;
            dayTimer.instance.NewDayTransition();
        }

        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (state == PlayerState.ACTIVE)
            {

                HelpWorker();
                if (joyOrDPad)
                {
                    ConvertToPos("HorizontalStick", "VerticalStick");
                    JoyStickMovement();
                    JoyMoveCoolDown();
                }
                else
                {
                    ConvertToPos("Horizontal", "Vertical");
                    Movement();
                    MoveCooldown();
                }
            }
            DeathCooler();
        }
        else if (GameStateManager.instance.GetState() == GameStates.STATE_FROGGER)
        {
            if (!froggerCompleted)
            {
                if (state == PlayerState.ACTIVE)
                {
                    if (joyOrDPad)
                    {
                        ConvertToPos("HorizontalStick", "VerticalStick");
                        JoyStickMovement();
                        JoyMoveCoolDown();
                    }
                    else
                    {
                        ConvertToPos("Horizontal", "Vertical");
                        Movement();
                        MoveCooldown();
                    }
                    if (!transform.parent)
                    {
                        if (transform.position.y > waterZone.x && transform.position.y < waterZone.y)
                        {
                            OriginalFroggerDeath(FroggerDeathType.DROWN);
                        }
                    }
                    else
                    {
                        Tile _tile = TileManager.instance.GetTile(transform.position);
                        if (_tile != tiles[0])
                        {
                            RemoveFromWorld();
                            AddToWorld();
                            _tile.Interaction(this);
                        }
                    }
                    if (transform.position.y > -4.5f)
                    {
                        coolDown = 0.0f;
                        froggerCompleted = true;
                        respawnParticles.transform.position = playerSpawn.position;
                        StartCoroutine("OriginalFroggerFinished");
                    }
                }
                DeathCooler();
            }
        }
    }

    private IEnumerator OriginalFroggerFinished()
    {
        while (true)
        {
            coolDown += Time.deltaTime;
            if (transform.position.x > 1.0f)
            {
                if (coolDown >= maxCool)
                {
                    coolDown = 0.0f;
                    transform.position = new Vector2(Mathf.Ceil(transform.position.x) - 1.5f, -4.0f);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                yield return null;
            }
            else if (transform.position.x < -1.0f)
            {
                if (coolDown >= maxCool)
                {
                    coolDown = 0.0f;
                    transform.position = new Vector2(Mathf.Floor(transform.position.x) + 1.5f, -4.0f);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                }
                yield return null;
            }
            else if (transform.position.y < -0.5f)
            {
                if (coolDown >= maxCool)
                {
                    coolDown = 0.0f;
                    transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                yield return null;
            }
            else
            {
                coolDown = 0.0f;
                dayTimer.instance.NewDayTransition();
                yield break;
            }
        }
    }

    void JoyStickMovement()
    {
        if ((Input.GetAxis("VerticalStick") != 0 || Input.GetAxis("HorizontalStick") != 0) && !MoveCooldown())
            anim.Play("PlayerWalk");

        float moveX = 0;
        float moveY = 0;

        if (hori > verti)
        {
            if (Input.GetAxis("HorizontalStick") != 0 && JoyMoveCoolDown())
            {
                moveX = Input.GetAxis("HorizontalStick") > 0 ? 1 : -1;
                //if (lastPos.x != moveX)
                {
                    Vector2 _newPos = new Vector2((transform.position.x + moveX), transform.position.y);
                    Tile _tile = TileManager.instance.GetTile(_newPos);
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _newPos;// _tile.transform.position;
                        RemoveFromWorld();
                        AddToWorld();
                        transform.SetParent(null);
                        _tile.Interaction(this);
                        if (Boss.instance.isActiveAndEnabled)
                        {
                            Boss.instance.ChasePlayer(_tile);
                        }
                    }

                    SoundManager.instance.playSound(0);
                    angle = Input.GetAxis("HorizontalStick") > 0 ? 270 : 90;
                    coolDown = 0;
                    lastPos.x = moveX;
                }
            }
        }
        else
        {
            if (Input.GetAxis("VerticalStick") != 0 && JoyMoveCoolDown())
            {
                moveY = Input.GetAxis("VerticalStick") > 0 ? 1 : -1;
             //   if (lastPos.y != moveY)
                {
                    Vector2 _newPos = new Vector2(transform.position.x, (transform.position.y + moveY));
                    Tile _tile = TileManager.instance.GetTile(_newPos);
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _newPos;// _tile.transform.position;
                        RemoveFromWorld();
                        AddToWorld();
                        transform.SetParent(null);
                        _tile.Interaction(this);
                        if (Boss.instance.isActiveAndEnabled)
                        {
                            Boss.instance.ChasePlayer(_tile);
                        }
                    }
                    SoundManager.instance.playSound(0);
                    angle = Input.GetAxis("VerticalStick") > 0 ? 0 : 180;
                    coolDown = 0;
                    lastPos.y = moveY;
                }
            }
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (Input.GetAxis("HorizontalStick") == 0)
        {
            lastPos.x = 0;
        }
        if (Input.GetAxis("VerticalStick") == 0)
        {
            lastPos.y = 0;
        }
    }

    void Movement()
    {
        if ((Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0) && !MoveCooldown())
            anim.Play("PlayerWalk");

        float moveX = 0;
        float moveY = 0;

        if (hori > verti)
        {
            if (Input.GetAxis("Horizontal") != 0 && MoveCooldown())
            {
                moveX = Input.GetAxis("Horizontal") > 0 ? 1 : -1;
                if (lastPos.x != moveX)
                {
                    Vector2 _newPos = new Vector2((transform.position.x + moveX), transform.position.y);
                    Tile _tile = TileManager.instance.GetTile(_newPos);
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _newPos;// _tile.transform.position;
                        RemoveFromWorld();
                        AddToWorld();
                        transform.SetParent(null);
                        _tile.Interaction(this);
                        if(Boss.instance.isActiveAndEnabled)
                        {
                            Boss.instance.ChasePlayer(_tile);
                        }
                    }

                    SoundManager.instance.playSound(0);
                    angle = Input.GetAxis("Horizontal") > 0 ? 270 : 90;
                    coolDown = 0;
                    lastPos.x = moveX;
                }
            }
        }
        else
        {
            if (Input.GetAxis("Vertical") != 0 && MoveCooldown())
            {
                moveY = Input.GetAxis("Vertical") > 0 ? 1 : -1;
                if (lastPos.y != moveY)
                {
                    Vector2 _newPos = new Vector2(transform.position.x, (transform.position.y + moveY));
                    Tile _tile = TileManager.instance.GetTile(_newPos);
                    if (_tile.CheckMovement(this))
                    {
                        transform.position = _newPos;// _tile.transform.position;
                        RemoveFromWorld();
                        AddToWorld();
                        transform.SetParent(null);
                        _tile.Interaction(this);
                        if (Boss.instance.isActiveAndEnabled)
                        {
                            Boss.instance.ChasePlayer(_tile);
                        }
                    }
                    SoundManager.instance.playSound(0);
                    angle = Input.GetAxis("Vertical") > 0 ? 0 : 180;
                    coolDown = 0;
                    lastPos.y = moveY;
                }
            }
        }
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (Input.GetAxis("Horizontal") == 0)
        {
            lastPos.x = 0;
        }
        if (Input.GetAxis("Vertical") == 0)
        {
            lastPos.y = 0;
        }
       
    }
    bool JoyMoveCoolDown()
    {
        if (coolDown< 0.8f)
        {
            coolDown += Time.deltaTime;
            return false;
        }
        return true;
    }

    bool MoveCooldown()
    {
        if (coolDown < maxCool)
        {
            coolDown += Time.deltaTime;
            return false;
        }
        return true;
    }

    //movement related function
    void ConvertToPos(string ho, string ve)
    {
        hori = Input.GetAxis(ho);
        verti = Input.GetAxis(ve);
        if (hori < 0)
        {
            hori -= hori * 2;
        }
        if (verti < 0)
        {
            verti -= verti * 2;
        }
    }

    public void Die()
    {
        if(!froggerCompleted)
        {
            OriginalFroggerDeath(FroggerDeathType.OFFSCREEN);
            return;
        }
        Boss.instance.EndChase();
        //   bloodSplatter.Play();
        state = PlayerState.DEAD;
        FrogCorpse frogCorpse = (FrogCorpse)Instantiate(corpse, transform.position, transform.rotation);
        frogCorpse.blood.transform.position = frogCorpse.transform.position;

        spriteRenderer.enabled = false;
        transform.position = playerSpawn.position;
        RemoveFromWorld();
        //Rather than this leave behind a corpse call remove from world, move position then add to world immediately
        strikes -= 1;
        StatTracker.instance.changeLifeCount(strikes);

        if (strikes ==0) //If the player has run out of lives
        {
            //Game over
            GameStateManager.instance.ChangeState(GameStates.STATE_GAMEOVER);
        }
        SoundManager.instance.playSound(splat);
    }

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Worker")
        {
            if (state == PlayerState.ACTIVE)
            {
                StatTracker.instance.totalDeaths++;
                StatTracker.instance.causeOfDeath.text = "A co-worker stepped on you";
                Die();
            }
        }
        else if(_obj.tag == "Boss")
        {
            if (state == PlayerState.ACTIVE)
            {
                StatTracker.instance.totalDeaths++;
                StatTracker.instance.bossDeaths++;
                StatTracker.instance.causeOfDeath.text = "Your boss stepped on you";
                Die();
            }
        }
        else if (_obj.tag == "FroggerObject")
        {
            if (state == PlayerState.ACTIVE)
            {
                if(((FroggerObject)_obj).kill)
                {
                    OriginalFroggerDeath(((FroggerObject)_obj).death);
                }
            }
        }
    }

    public void OriginalFroggerDeath(FroggerDeathType _deathType)
    {
        state = PlayerState.DEAD;
        spriteRenderer.enabled = false;
        RemoveFromWorld();

        //Reset posiiton and play appropriate sound
        //Play appropriate sound
        switch(_deathType)
        {
            case FroggerDeathType.RUNOVER:
                FrogCorpse frogCorpse = (FrogCorpse)Instantiate(corpse, transform.position, transform.rotation);
                frogCorpse.blood.transform.position = frogCorpse.transform.position;
                SoundManager.instance.playSound(splat);
                break;
            case FroggerDeathType.CROCO:
                ParticleSystem p = Instantiate(bloodSplatter);
                p.transform.position = transform.position;
                p.Play();
                SoundManager.instance.playSound(splat);
                break;
            case FroggerDeathType.DROWN:
                SoundManager.instance.playSound(splash);
                break;
        }
        transform.SetParent(null);
        transform.position = froggerSpawn.position;
    }

    void DeathCooler()
    {
        if (state == PlayerState.DEAD)
        {
            if (deathCool < maxDeathcool)
            {
                if (deathCool > maxDeathcool - .8f && !respawnParticles.isPlaying)
                {
                    respawnParticles.Play();
                }
                deathCool += Time.deltaTime;
            }
            else
            {
                anim.SetBool("Dead", false);
                spriteRenderer.enabled = true;
                state = PlayerState.ACTIVE;
                deathCool = 0;
                //transform.position = new Vector2(-0.5f, -1.0f);
                angle = 0;
                AddToWorld();
            }
        }
    }
    void HelpWorker()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Tile _tile = TileManager.instance.GetTile(transform.position);
            _tile.Interaction(this);
        }
    }

    public List<Tile> GetRouteToPlayer()
    {
        List<Tile> _tiles = new List<Tile>();

        int _x;
        int _y;
        int _direction;

        //Loop through objects to see if player is in cubicle
        foreach (WorldObject _wo in tiles[0].GetObjects())
        {
            if(_wo.tag == "Cubicle")
            {
                //Create a tile route into the cubicle
                List<Tile> _temp = ((Cubicle)_wo).GetTileRoute(tiles[0]);
                //Get the closest spawner
                _tiles.Add(TileManager.instance.GetAssociatedSpawner(_temp[0]));

                _x = _tiles[0].X();
                _y = _tiles[0].Y();
                _direction = _x < _temp[0].X() ? 1 : -1;

                //Loop and create route from spawner to entrance of cubicle
                while (true)
                {
                    _x += _direction;
                    Tile _tempTile = TileManager.instance.GetTile(_x, _y);
                    if(_tempTile.X() == _temp[0].X())
                    {
                        break;
                    }
                    _tiles.Add(_tempTile);
                }
                //Add the cubicle route after the entrance
                _tiles.AddRange(_temp);
                return _tiles;
            }
        }

        _tiles.Add(TileManager.instance.GetAssociatedSpawner(tiles[0]));
        _x = _tiles[0].X();
        _y = _tiles[0].Y();
        _direction = _x < TileManager.instance.GetTile(tiles[0].X(), _y).X() ? 1 : -1;

        //Create a route from the spawner to the tile where direction shoul dbe changed
        while (true)
        {
            _x += _direction;
            Tile _tempTile = TileManager.instance.GetTile(_x, _y);
            _tiles.Add(_tempTile);
            if (_tempTile.X() == tiles[0].X())
            {
                _direction = _y < tiles[0].Y() ? 1 : -1;

                while (tiles[0].Y() != _tiles[_tiles.Count - 1].Y())
                {
                    _y += _direction;
                    _tempTile = TileManager.instance.GetTile(_x, _y);
                    _tiles.Add(_tempTile);
                }
                break;
            }
            
        }
        return _tiles;
    }

    public override void Reset()
    {
        coolDown = 0.0f;
        deathCool = 0.0f;
        lastPos = Vector2.zero;
        state = PlayerState.ACTIVE;
        if(tiles[0])
        {
            RemoveFromWorld();
        }
        transform.position = playerSpawn.position;
        anim.SetBool("Dead", false);
        spriteRenderer.enabled = true;
        AddToWorld();
    }
}


public enum PlayerState
{
    ACTIVE,
    DEAD,
}
