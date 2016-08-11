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
    private Cubicle[] cubicles = null;
    
    public SkinHairCombo[] skinHairCombos;

    private const int numSeats = 40;

    private void Awake()
    {
        workerManager = this;
        Worker.numPeopleNeedHelp = 0;
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
        Worker.numPeopleNeedHelp = 0;
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
        TileManager.instance.DefaultSpawnerLanes();
    }
    
    [System.Serializable]
    public class SkinHairCombo
    {
        public Color Skintone;
        public Sprite[] hairSprites;
    }

    public Worker GetPooledWorker(bool _unique = false)
    {
        Worker _worker = workerPool.GetPooledObject();
        if (!_worker.GetIsSetup())
        {
            Worker.workerType _type = Worker.workerType.Standard;
            bool sex=false,big=false;
            Sprite hair=null;
            if (_unique)
            {
                float _rand = Random.value;
                _type = _rand < 0.35f ? Worker.workerType.Trolley : _type;
                _type = _rand < 0.25f ? Worker.workerType.Spinning : _type;
                _type = _rand < 0.15f ? Worker.workerType.Janitor : _type;
            }
            if (_type == Worker.workerType.Standard)
            {
                if (Random.value >= .5f)
                    sex = true;
                if (Random.value >= .8f)
                    big = true;
                    
            }
            //Set up this bad boy
            //...or bad girl, Spamphibian is an equal opportunity video game.

            //WELL AT LEAST I TRIED THIS METHOD

            //Color Skin = Random.ColorHSV(0.073f, 0.076f, 0.5f, 0.85f, 0.4f, 0.9f, 1, 1);
            Color Skin = Random.ColorHSV(0.105f, 0.085f, 0.177f, 0.72f, 0.38f, 0.94f, 1, 1);
            Color Clothes = Random.ColorHSV(0,1,.5f,1,.35f,1);
            Color Eyes = Random.ColorHSV(.35f, .7f, .65f, 1, .65f, 1);
            Eyes = Color.black;
            SkinHairCombo shc = skinHairCombos[Random.Range(0, skinHairCombos.Length)];
            Skin = shc.Skintone;
            hair = shc.hairSprites[Random.Range(0, shc.hairSprites.Length)];
            
            _worker.SetupWorker(Skin, hair,sex,Clothes,Eyes,_type,big);
        }
        return _worker;
    }
}
