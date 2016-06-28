using UnityEngine;
using System.Collections;

public class Spawner : WorldObject
{
    private float cooldown = 0.0f;
    private float spawnCooldown;

    [SerializeField] private float minSpawnCooldown = 1.5f;
    [SerializeField] private float maxSpawnCooldown = 2.4f;

    [SerializeField] private bool isLeft = false;
    [SerializeField] private float laneSpeed = 5.0f;

    [SerializeField] private bool isActive = false;

	protected override void Awake ()
    {
        base.Awake();
        spawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
	}
	

	private void Update ()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        {
            if (isActive)
            {
                cooldown = cooldown + Time.deltaTime < spawnCooldown ? cooldown + Time.deltaTime : spawnCooldown;
                if (cooldown == spawnCooldown)
                {
                    Spawn();
                }
            }
        }
	}


    private void Spawn()
    {
        Worker _worker = WorkerManager.instance.GetPooledWorker(true);
        _worker.transform.position = transform.position;
        if(isLeft)
        { 
            _worker.Initialise(Vector3.right, laneSpeed);
        }
        else
        {
            _worker.Initialise(Vector3.left, laneSpeed);
        }

        cooldown = 0;
        spawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
    }

    public Vector3 GetDirection()
    {
        return isLeft ? Vector3.right : Vector3.left;
    }

    public float GetSpeed()
    {
        return laneSpeed;
    }

    public void ModifySpawner(float _min, float _max, float _speed)
    {
        minSpawnCooldown *= _min;
        maxSpawnCooldown *= _max;
        laneSpeed *= _speed;
        spawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
    }

    public override void Reset()
    {
        cooldown = 0.0f;
    }
}
