using UnityEngine;
using System.Collections;

public class multiplayerManager : MonoBehaviour {

    public int numberOfPlayers = 0;
    public static multiplayerManager instance;
    public int currentActivePlayer=0; //0 FOR P1, 1 FOR P2
    //Only do intro zoom + introduction email for 1 player
    //Only show game over IMAGE for when both players fail

	void Start ()
    {
        instance = this;
	}
	
	void Update ()
    {
	
	}
}
