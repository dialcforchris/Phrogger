using UnityEngine;
using System.Collections;

public class WorkerManager : MonoBehaviour
{
    private ObjectPool<Worker> workerPool = null;
    [SerializeField] private Worker workerPrefab = null;

    private void Awake()
    {
        workerPool = new ObjectPool<Worker>(workerPrefab, 50);
    }
}
