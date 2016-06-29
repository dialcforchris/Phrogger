using UnityEngine;
using System.Collections;

public class FroggerState : GameState
{
    public override void OnStateActivate()
    {

    }

    public override void OnStateDeactivate()
    {
        
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            GameStateManager.instance.ChangeState(GameStates.STATE_PAUSE);
    }
}
