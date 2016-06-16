using UnityEngine;
using System.Collections;

public class WorkerManager : MonoBehaviour
{
    private static WorkerManager workerManager = null;
    public static WorkerManager instance { get { return workerManager; } }

    private ObjectPool<Worker> workerPool = null;
    [SerializeField] private Worker workerPrefab = null;

    [SerializeField] private Sprite[] bodySprites = null;
    [SerializeField] private Sprite[] hairSprites = null;

    private void Awake()
    {
        workerManager = this;
        workerPool = new ObjectPool<Worker>(workerPrefab, 50);
    }

    public Worker GetPooledWorker()
    {
        Worker _worker = workerPool.GetPooledObject();
        if (!_worker.GetIsSetup())
        {
            _worker.SetupWorker(bodySprites[Random.Range(0, bodySprites.Length - 1)].name, hairSprites[Random.Range(0, hairSprites.Length - 1)]);
        }
        return _worker;
    }
}
