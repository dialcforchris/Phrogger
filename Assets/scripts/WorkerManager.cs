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

    public void SetupDefaultPositions(int _seated = 30)
    {
        int _numSeatsSpawned = 0;
        int[] _cubicleNumbers = new int[_seated];
        for(int i = 0; i < _seated; ++i)
        {
            _cubicleNumbers[i] = Random.Range(0, 31);
        }
        _cubicleNumbers = _cubicleNumbers.Distinct().ToArray();

        List<Worker> _workers = new List<Worker>();

        int _help = Random.Range(0, _cubicleNumbers.Length);

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
                    if(_numSeatsSpawned == _help)
                    {
                        _worker.StateSwitch(WorkerState.HELP);
                    }
                    _cubicleNumbers[i] = Random.Range(0, 2) == 0 ? _cubicleNumbers[i] : cubicles.Length;
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

    public Worker GetPooledWorker(bool _unique = false)
    {
        Worker _worker = workerPool.GetPooledObject();
        if (!_worker.GetIsSetup())
        {
            string _s = bodySprites[Random.Range(0, bodySprites.Length)];
            if (_unique)
            {
                float _rand = Random.value;
                _s = _rand < 0.10f ? "Janitor" : _s;
                _s = _rand < 0.25f ? "Spinning chairman" : _s;
            }
            _worker.SetupWorker(_s, hairSprites[Random.Range(0, hairSprites.Length)]);
        }
        return _worker;
    }
}
  