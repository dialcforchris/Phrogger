using UnityEngine;
using System.Collections;

public class FroggerPlatform : FroggerObject
{
    [SerializeField]
    private bool isCrocodile = false;
    [SerializeField]
    private bool isTurtle = false;
    private bool turtleDropped = false;
    private bool canDrop = false;
    [SerializeField]
    private Animator[] turtles = null;

    private float cooldown = 0.0f;
    [SerializeField]
    private float minTurtle = 3.0f;
    [SerializeField]
    private float maxTurtle = 15.0f;
    private float turtleDrop;

    public override void Initialise(Vector3 _dir, float _speed)
    {
        gameObject.SetActive(true);
        direction = _dir;
        speed = _speed;
        AddToWorld();
        turtleDrop = Random.Range(minTurtle, maxTurtle);
        cooldown = Random.Range(0.0f, turtleDrop);
        canDrop = Random.Range(0, 2) == 0 ? true : false;
    }

    public override void Interaction(WorldObject _obj)
    {
        if (_obj.tag == "Player")
        {
            if (killOnTouch)
            {
                ((Player)_obj).OriginalFroggerDeath(deathType);
            }
            else if (isCrocodile)
            {
                if (_obj.GetTile(0) == tiles[0])
                {
                    ((Player)_obj).OriginalFroggerDeath(deathType);
                }
                else
                {
                    _obj.transform.SetParent(transform);
                }
            }
            else if (turtleDropped)
            {
                if (spriteRenderer.color.a > 0.1f)
                {
                    _obj.transform.SetParent(transform);
                }
            }
            else
            {
                if (_obj.transform.position.y == transform.position.y)
                {
                    _obj.transform.SetParent(transform);
                }
            }

        }
    }

    protected override void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_FROGGER || GameStateManager.instance.GetState() == GameStates.STATE_SPLASH)
        {
            base.Update();
            if (isTurtle)
            {
                if (canDrop)
                {
                    if (!turtleDropped)
                    {
                        cooldown += Time.deltaTime;
                        if (cooldown >= turtleDrop)
                        {
                            turtleDropped = true;
                            foreach (Animator _a in turtles)
                            {
                                _a.SetTrigger("Dive");
                            }
                        }
                    }
                    else if (spriteRenderer.color.a < 0.1f)
                    {
                        if (cooldown >= turtleDrop)
                        {
                            cooldown = 0.0f;
                            foreach (Tile _t in tiles)
                            {
                                foreach (WorldObject _wo in _t.GetObjects())
                                {
                                    if (_wo.tag == "Player")
                                    {
                                        Player.instance.transform.SetParent(null);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else if (turtles[0].GetCurrentAnimatorStateInfo(0).IsName("TurtleIdle"))
                    {
                        foreach (Animator _a in turtles)
                        {
                            _a.ResetTrigger("Dive");
                        }
                        turtleDrop = Random.Range(minTurtle, maxTurtle);
                        turtleDropped = false;
                    }
                }
            }
        }
    }

    public override void Reset()
    {
        --length;
        if (length == 0)
        {
            length = pivots.Length;

            turtleDropped = false;
            cooldown = 0.0f;
            foreach (Animator _a in turtles)
            {
                _a.ResetTrigger("Dive");
            }

            gameObject.SetActive(false);
            poolData.ReturnPool(this);
        }
    }
}