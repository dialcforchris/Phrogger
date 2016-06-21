using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        
    }

    private void Start()
    {
        InitialiseOffice();
    }

    private void InitialiseOffice()
    {
        WorkerManager.instance.SetupDefaultPositions();
    }
}
