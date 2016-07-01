using UnityEngine;
using System.Collections;

public class DestroyParticle : MonoBehaviour 
{
    [SerializeField]
    float maxTime=2;
    float timer = 0;
    void Update()
    {
        if (GameStateManager.instance.GetState() == GameStates.STATE_FROGGER || GameStateManager.instance.GetState() == GameStates.STATE_GAMEPLAY)
        timer += Time.deltaTime;
        if (timer>=maxTime)
        {
            Destroy(gameObject);
        }
    }

}
