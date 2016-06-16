using UnityEngine;
using System.Collections;

public class Spawner : WorldObject
{
    [SerializeField] private float cooldown = 0.0f;
    [SerializeField] private float spawnCooldown;

    [SerializeField] private float minSpawnCooldown = 1.5f;
    [SerializeField] private float maxSpawnCooldown = 2.4f;

    [SerializeField] private bool isLeft = false;
    [SerializeField] private float laneSpeed = 5.0f;

	void Awake ()
    {
        spawnCooldown = Random.Range(minSpawnCooldown, maxSpawnCooldown);
	}
	

	private void Update ()
    {
        cooldown = cooldown + Time.deltaTime < spawnCooldown ? cooldown + Time.deltaTime : spawnCooldown;
        if(cooldown == spawnCooldown)
        {
            Spawn();
        }
	}


    void Spawn()
    {
        Worker _worker = WorkerManager.instance.GetPooledWorker();
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
}
