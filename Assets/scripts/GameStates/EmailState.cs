using UnityEngine;
using System.Collections;

public class EmailState : GameState
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

public class GameOverState : GameState
{
    public override void OnStateActivate()
    {
        SoundManager.instance.music.volume = 0;
        StatTracker.instance.StartCoroutine(StatTracker.instance.GameOverUIReveal());
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}

public class DayOverState : GameState
{
    public override void OnStateActivate()
    {
    }

    public override void OnStateDeactivate()
    {

    }

    public override void Update()
    {
    }
}
