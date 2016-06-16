using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour 
{
    int maxRounds = 3;
    int currentRound = 0;

    float roundTimer = 60;
    float currentRoundTimer = 0;
    public Player player;

    private static GameLogic theGameLogic = null;
    public static GameLogic instance
    {
        get { return theGameLogic; }
    }

    // Use this for initialization
	void Start () 
    {
	    if (theGameLogic == null)
        {
            theGameLogic = this;
        }
      
	}
	
	// Update is called once per frame
	void Update () 
    {
	    
	}

    void TimerCountdown()
    {
        if (currentRoundTimer<roundTimer)
        {
            currentRoundTimer += Time.deltaTime;
        }
        else
        {
            currentRoundTimer = 0;
            currentRound++;
            player.Die();
            RoundCount();
        }
    }
    void RoundCount()
    {
        if (currentRound==maxRounds)
        {
            //end game
        }
    }

    public void ResetGame()
    {
        currentRoundTimer = 0;
        currentRound = 0;
    }
}
