using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorkerManager : MonoBehaviour
{
    private static WorkerManager workerManager = null;
    public static WorkerManager instance { get { return workerManager; } }

    private ObjectPool<Worker> workerPool = null;
    [SerializeField] private Worker workerPrefab = null;

    [SerializeField] private string[] bodySprites = null;
    [SerializeField] private Sprite[] hairSprites = null;
    [SerializeField] private List<Cubicle> cubicles = new List<Cubicle>();
   
    private void Awake()
    {
        workerManager = this;
        workerPool = new ObjectPool<Worker>(workerPrefab, 50,transform);
        Cubicle[] c = Cubicle.FindObjectsOfType<Cubicle>();
        foreach (Cubicle cub in c)
        {
           cubicles.Add(cub); 
            if (cub.chair>1)
            {
                cubicles.Add(cub);
            }
           
        }
        GiveCubicleID();
    }

    public Worker GetPooledWorker()
    {
        Worker _worker = workerPool.GetPooledObject();
        _worker.transform.parent = this.transform;
        if (!_worker.GetIsSetup())
        {
            AssignCubicle(_worker);
            _worker.SetupWorker(bodySprites[Random.Range(0, bodySprites.Length)],hairSprites[Random.Range(0, hairSprites.Length)]);
        }
        return _worker;
    }

    void AssignCubicle(Worker w)
    {
        if (Random.value>0.7f)
        {
            if (cubicles.Count>0)
            {
                int setdeskID = Random.Range(0, cubicles.Count);
                w.cubicleId = cubicles[setdeskID].cubicleId;
                cubicles.RemoveAt(setdeskID);
            }
        }
    }

    void GiveCubicleID()
    {
        for (int i = 0; i < cubicles.Count;i++ )
        {
            cubicles[i].cubicleId = i+1;
        }
    }
}
