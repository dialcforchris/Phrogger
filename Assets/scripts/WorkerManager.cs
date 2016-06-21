using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WorkerManager : MonoBehaviour
{
    private static WorkerManager workerManager = null;
    public static WorkerManager instance { get { return workerManager; } }

    private ObjectPool<Worker> workerPool = null;

    [SerializeField]
    private Worker workerPrefab = null;

    [SerializeField]
    private string[] bodySprites = null;
    [SerializeField]
    private Sprite[] hairSprites = null;
    [SerializeField]
    private Cubicle[] cubicles = null;

    private int cubicleIndex = 0;
    private int chairIndex = 0;

    private const int numSeats = 40;

    private void Awake()
    {
        workerManager = this;
        workerPool = new ObjectPool<Worker>(workerPrefab, 50, transform);
        cubicles = FindObjectsOfType<Cubicle>();

        List<Worker> _workers = new List<Worker>();
        for (int i = 0; i < cubicles.Length; ++i)
        {
            cubicles[i].cubicleId = i;
            for (int j = 0; j < cubicles[i].GetChairs(); ++j)
            {
                Worker _worker = GetPooledWorker();
                _worker.SetupCubicle(i, j);
                cubicles[i].AssignWorkerPositionData(_worker);
                _workers.Add(_worker);
            }
        }
        foreach (Worker _w in _workers)
        {
            _w.ReturnPool();
        }
    }

    public void SetupDefaultPositions(int _seated = 20)
    {
        int _numSeatsSpawned = 0;
        int[] _cubicleNumbers = new int[_seated];
        for(int i = 0; i < _seated; ++i)
        {
            _cubicleNumbers[i] = Random.Range(0, 31);
        }
        _cubicleNumbers = _cubicleNumbers.Distinct().ToArray();

        List<Worker> _workers = new List<Worker>();

        while (_numSeatsSpawned < _cubicleNumbers.Length)
        {
            Worker _worker = GetPooledWorker();
            bool _match = false;
            for (int i = 0; i < _cubicleNumbers.Length; ++i)
            {
                if (_worker.cubicleId == _cubicleNumbers[i])
                {
                    ++_numSeatsSpawned;
                    _worker.InitialiseToCubicle(cubicles[_worker.cubicleId].GetAssociatedSpawner().GetDirection(), cubicles[_worker.cubicleId].GetAssociatedSpawner().GetSpeed());
                    cubicles[_worker.cubicleId].AssignWorkerImmediately(_worker);
                    _match = true;
                    if(_numSeatsSpawned == 1)
                    {
                        _worker.StateSwitch(WorkerState.HELP);
                    }
                    break;
                }
            }
            if (!_match)
            {
                _workers.Add(_worker);
            }
        }

        foreach (Worker _w in _workers)
        {
            _w.ReturnPool();
        }
    }

    public Worker GetPooledWorker()
    {
        Worker _worker = workerPool.GetPooledObject();
        if (!_worker.GetIsSetup())
        {
            _worker.SetupWorker(bodySprites[Random.Range(0, bodySprites.Length)], hairSprites[Random.Range(0, hairSprites.Length)]);
        }
        return _worker;
    }
}
  