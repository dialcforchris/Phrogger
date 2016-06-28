using UnityEngine;
using System.Collections;

public class FroggerSpawner : WorldObject
{
    private ObjectPool<FroggerObject> objectPool = null;

    [SerializeField] private float minSpawnRate = 1.5f;
    [SerializeField] private float maxSpawnRate = 2.5f;
    private float spawnRate;
    private float cooldown = 0.0f;

    [SerializeField] private float laneSpeed = 5.0f;

    [SerializeField] private FroggerObject spawnObject = null;

    [SerializeField] private bool isLeft = false;

    

    protected override void Awake()
    {
        base.Awake();
        objectPool = new ObjectPool<FroggerObject>(spawnObject, 5, transform);
    }

    private void Update()
    {
        if(GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            cooldown += Time.deltaTime;
            if(cooldown >= spawnRate)
            {
                cooldown = 0.0f;
                spawnRate = Random.Range(minSpawnRate, maxSpawnRate);
                FroggerObject _obj = objectPool.GetPooledObject();
                _obj.transform.position = transform.position;
                _obj.Initialise(isLeft ? Vector3.right : Vector3.left, laneSpeed);
            }
        }
    }
}
