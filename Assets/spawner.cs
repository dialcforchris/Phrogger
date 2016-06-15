using UnityEngine;
using System.Collections;

public class spawner : MonoBehaviour
{
    float cool;
    float maxCool = 1;
    public GameObject toSpawn;
    // Use this for initialization
	void Start () {
        maxCool = Random.Range(1.5f, 2.4f);
	}
	
	// Update is called once per frame
	void Update () {

        
        Cool();
	}
    void Cool()
{
    if (cool<maxCool)
    {
        cool += Time.deltaTime;
    }
    else
    {
        Spawn();
    }
}
    void Spawn()
    {
        Instantiate(toSpawn, transform.position, Quaternion.identity);
        cool = 0;
       // maxCool = Random.Range(1.5, 2.4f);
    }
}
