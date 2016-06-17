using UnityEngine;

public enum GameStates
{
    STATE_GAMEPLAY = 0,
    STATE_PAUSE,
    STATE_EMAIL,
    GAMESTATES_COUNT
}

public class GameStateManager : MonoBehaviour
{
    private static GameStateManager singleton = null;
    public static GameStateManager instance { get { return singleton; } }

    private GameState[] states = new GameState[(int)GameStates.GAMESTATES_COUNT];
    private GameStates currentState = GameStates.STATE_GAMEPLAY;

    private void Awake()
    {
        if (singleton)
        {
            DestroyImmediate(this);
        }
        else
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);
            states[(int)GameStates.STATE_GAMEPLAY] = new GameplayState();
            states[(int)GameStates.STATE_PAUSE] = new PauseState();
            states[(int)GameStates.STATE_EMAIL] = new EmailState();
        }
    }

    private void Update()
    {
        states[(int)currentState].Update();
    }

	public void ChangeState(GameStates _state)
    {
        states[(int)currentState].OnStateDeactivate();
        currentState = _state;
        states[(int)currentState].OnStateActivate();
    }

    public GameStates GetState()
    {
        return currentState;
    }


}
