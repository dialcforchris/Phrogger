using UnityEngine;
using System.Collections;

public class FroggerSpawner : WorldObject
{
    private ObjectPool<FroggerObject> objectPool = null;
    private ObjectPool<FroggerObject> objectRarePool = null;

    [SerializeField] private float minSpawnRate = 1.5f;
    [SerializeField] private float maxSpawnRate = 2.5f;
    private float spawnRate;
    private float cooldown = 0.0f;

    [SerializeField] private float laneSpeed = 5.0f;

    [SerializeField] private FroggerObject spawnObject = null;
    [SerializeField] private FroggerObject rareObject = null;

    [SerializeField] private bool isLeft = false;

    

    protected override void Awake()
    {
        base.Awake();
        objectPool = new ObjectPool<FroggerObject>(spawnObject, 5, transform);
        if (rareObject)
        {
            objectRarePool = new ObjectPool<FroggerObject>(rareObject, 2, transform);
        }
    }

    protected override void Start()
    {
        base.Start();
        for (int i = 1; i <  Mathf.Floor((19/laneSpeed) /  maxSpawnRate); ++i)
        {
            FroggerObject _obj = objectPool.GetPooledObject();
            _obj.transform.position = transform.position;
            _obj.transform.position += ((((isLeft ? Vector3.right : Vector3.left) * laneSpeed * Time.deltaTime) * Mathf.Floor(maxSpawnRate / Time.deltaTime)) * i);
            _obj.Initialise(isLeft ? Vector3.right : Vector3.left, laneSpeed);
        }
    }

    private void Update()
    {
        if(GameStateManager.instance.GetState() == GameStates.STATE_FROGGER)
        {
            cooldown += Time.deltaTime;
            if(cooldown >= spawnRate)
            {
                cooldown = 0.0f;
                spawnRate = Random.Range(minSpawnRate, maxSpawnRate);
                FroggerObject _obj;
                if (!rareObject)
                {
                    _obj = objectPool.GetPooledObject();
                }
                else
                {
                    if(Random.Range(0, 10) < 2) //20%
                    {
                        _obj = objectRarePool.GetPooledObject();
                    }
                    else
                    {
                        _obj = objectPool.GetPooledObject();
                    }
                }
                _obj.transform.position = transform.position;
                _obj.Initialise(isLeft ? Vector3.right : Vector3.left, laneSpeed);
            }
        }
    }
}
